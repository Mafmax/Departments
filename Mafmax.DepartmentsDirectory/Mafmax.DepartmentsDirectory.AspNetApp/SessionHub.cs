using Mafmax.DepartmentsDirectory.AspNetApp.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace Mafmax.DepartmentsDirectory.AspNetApp
{
    public class SessionHub : Hub
    {
        private static List<User> connectedUsers = new List<User>();
        private static List<string> groups = new List<string>();
        private void UpdateData(CompanyEntity[] newData)
        {
            using (var context = new DDContext())
            {
                var companies = context.Companies.ToArray();
                for (int i = 0; i < companies.Length; i++)
                {
                    RemoveCompany(companies[i].Id);
                }
                foreach (var company in newData)
                {
                    context.Companies.Add(company);
                    foreach (var department in company.Departments)
                    {
                        context.Departments.Add(department);
                        foreach (var group in department.Groups)
                        {
                            context.Groups.Add(group);
                        }
                    }
                }
                context.SaveChanges();
            }
        }
        public void DownloadXml()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(CompanyEntity[]));
            string rawName = $"Departments directory data. {DateTime.Now.ToString().Replace(':', '-')}.xml";
            using (var context = new DDContext())
            {
                context.Groups.Load();
                context.Departments.Load();
                context.Companies.Load();
                var data = context.Companies.ToArray();
                using (var stream = new StringWriter())
                {
                    formatter.Serialize(stream, data);
                    var xml = stream.ToString();
                    Clients.Caller.onDownloadXml(rawName, xml);
                }
            }

        }
        public void UploadXml(string input)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(CompanyEntity[]));
            CompanyEntity[] data = null;
            using (var stream = new StringReader(input))
            {
                try
                {
                    data = (CompanyEntity[])formatter.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    Clients.Caller.onMessage("Ошибка!!! Не удалось импортировать данные.");
                }
            }
            UpdateData(data);
            OnStartPage();
        }

        public void ReplaceDepartment(int id, string newCompanyId)
        {
            if (!int.TryParse(newCompanyId, out int companyId))
            {
                Clients.Caller.onMessage("Ошибка!!! Номер компании должен быть числом");
                return;
            }
            using (var context = new DDContext())
            {
                context.Departments.Include(x => x.Company).Load();
                context.Departments.Include(x => x.Groups).Load();
                var department = context.Departments.Find(id);
                var newCompany = context.Companies.Find(companyId);
                if (newCompany is null)
                {
                    Clients.Caller.onMessage("Ошибка!!! Компания с таким номером не найдена");
                    return;
                }
                Clients.Group($"Company{department.Company.Id}").onRemoveDepartment(department);
                Clients.Group($"Company{newCompany.Id}").onAddDepartment(department);
                Clients.Caller.onMessage("Успешно!");
                department.Company = newCompany;
                context.SaveChanges();
            }
        }
        public void EditDepartment(int id, string newName)
        {
            using (var context = new DDContext())
            {
                context.Companies.Load();
                var department = context.Departments.Find(id);
                department.Name = newName;
                context.SaveChanges();
                Clients.Group($"Company{department.Company.Id}").onEditDepartment(department);
            }
        }
        public void RemoveDepartment(int id)
        {
            using (var context = new DDContext())
            {
                context.Companies.Include(x => x.Departments).Load();
                context.Groups.Load();
                var department = context.Departments.Find(id);
                var companyId = department.Company.Id;
                context.Departments.Remove(department);
                context.SaveChanges();
                Clients.Group($"Company{companyId}").onRemoveDepartment(department);
            }
        }
        public void AddDepartment(int companyId, string name)
        {
            using (var context = new DDContext())
            {
                context.Companies.Include(x => x.Departments).Load();
                var company = context.Companies.Find(companyId);
                var department = new DepartmentEntity();
                department.Name = name;
                company.Departments.Add(department);
                context.SaveChanges();
                Clients.Group($"Company{companyId}").onAddDepartment(department);
            }
        }

        public void ReplaceGroup(int groupId, string newDepartmentId)
        {
            using (var context = new DDContext())
            {
                if (!int.TryParse(newDepartmentId, out int departmentId))
                {
                    Clients.Caller.onMessage("Ошибка!!! Номер департамента должен быть числом.");
                    return;
                }

                context.Companies.Include(x => x.Departments).Load();
                var newDepartment = context.Departments.Find(departmentId);
                if (newDepartment is null)
                {
                    Clients.Caller.onMessage("Ошибка!!! Указанный департамент не найден.");
                    return;
                }
                var group = context.Groups.Find(groupId);
                if (newDepartment.Company != group.Department.Company)
                {
                    Clients.Caller.onMessage("Ошибка!!! Департамент находится в другой компании.\nВ другую компанию можно перемещать только департаменты.");
                    return;
                }

                group.Department = newDepartment;
                context.SaveChanges();
                Clients.Group($"Company{group.Department.Company.Id}").onReplaceGroup(group, group.Department.Id);
                Clients.Caller.onMessage("Успешно!");

            }
        }
        public void EditGroup(int groupId, string newName)
        {

            using (var context = new DDContext())
            {
                context.Companies.Include(x => x.Departments).Load();
                var group = context.Groups.Find(groupId);
                group.Name = newName;
                context.SaveChanges();
                Clients.Group($"Company{group.Department.Company.Id}").onEditGroup(group);
            }
        }
        public void AddGroup(int departmentId, string name)
        {
            using (var context = new DDContext())
            {
                context.Companies.Include(x => x.Departments).Load();
                context.Groups.Load();
                var department = context.Departments.Find(departmentId);
                var group = new GroupEntity();
                group.Name = name;
                department.Groups.Add(group);
                context.SaveChanges();
                Clients.Group($"Company{department.Company.Id}").onAddGroup(group, group.Department.Id);
            }
        }
        public void RemoveGroup(int groupId)
        {
            using (var context = new DDContext())
            {
                context.Companies.Include(x => x.Departments).Load();
                var group = context.Groups.Find(groupId);
                var companyId = group.Department.Company.Id;
                context.Groups.Remove(group);
                context.SaveChanges();
                Clients.Group($"Company{companyId}").onRemoveGroup(groupId);
            }
        }
        public void StartProcessingCompany(int id)
        {
            var group = $"Company{id}";
            if (!groups.Contains(group)) groups.Add(group);
            var connectionId = Context.ConnectionId;
            Groups.Add(connectionId, group);
            using (var context = new DDContext())
            {
                context.Companies.Include(x => x.Departments).Load();
                context.Groups.Load();
                var company = context.Companies.Find(id);
                Clients.Caller.onCompanyProcessingPage(company);
            }
        }
        public void RemoveCompany(int id)
        {
            using (var context = new DDContext())
            {
                context.Departments.Include(x => x.Groups).Load();
                var company = context.Companies.Find(id);
                context.Companies.Remove(company);
                context.SaveChanges();
                Clients.Clients(GetIds(UserSessionState.Start)).onCompanyRemoved(company);
                Clients.Group($"Company{id}").onCurrentCompanyRemoved();
            }
        }
        public void ChangeCompany(int id, string newName)
        {
            using (var context = new DDContext())
            {
                var company = context.Companies.Find(id);
                company.Name = newName;
                context.SaveChanges();
                Clients.Clients(GetIds(UserSessionState.Start)).onCompanyChanged(company);
                Clients.Group($"Company{id}").onCurrentCompanyChanged(company);
            }
        }
        public void AddCompany(string name)
        {

            using (var context = new DDContext())
            {
                var company = new CompanyEntity();
                company.Name = name;
                context.Companies.Add(company);
                context.SaveChanges();
                var ids = GetIds(UserSessionState.Start);
                Clients.Clients(ids).onCompanyAdded(company);
            }
        }
        public void OnStartPage()
        {
            var connectionId = Context.ConnectionId;
            foreach (var group in groups)
            {
                Groups.Remove(connectionId, group);
            }
            var user = connectedUsers.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (user is null)
            {
                user = new User();
                user.ConnectionId = connectionId;
                connectedUsers.Add(user);
            }
            user.SessionState = UserSessionState.Start;

            using (var context = new DDContext())
            {
                var cmps = context.Companies.ToList();
                Clients.Caller.onStartPage(context.Companies);
            }
        }
        private List<string> GetIds(UserSessionState sessionState)
        {
            return connectedUsers
                 .Where(x => x.SessionState == sessionState)
                 .Select(x => x.ConnectionId)
                 .ToList();
        }
    }

}