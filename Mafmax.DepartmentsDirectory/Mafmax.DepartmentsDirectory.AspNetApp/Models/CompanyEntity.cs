using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Mafmax.DepartmentsDirectory.AspNetApp.Models
{
    [Table("Companies")]
    [Serializable]
    public class CompanyEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public  List<DepartmentEntity> Departments { get; set; }
        public CompanyEntity()
        {
            Departments = new List<DepartmentEntity>();
        }
        public static bool operator ==(CompanyEntity c1, CompanyEntity c2)
        {
            return c1.Id == c2.Id;
        }
        public static bool operator !=(CompanyEntity c1, CompanyEntity c2)
        {
            return c1.Id != c2.Id;
        }
    }
}