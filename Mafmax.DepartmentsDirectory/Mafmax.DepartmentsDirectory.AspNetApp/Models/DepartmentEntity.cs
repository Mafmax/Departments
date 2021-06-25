using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Mafmax.DepartmentsDirectory.AspNetApp.Models
{
    [Table("Departments")]
    [Serializable]
    public class DepartmentEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GroupEntity> Groups { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public CompanyEntity Company { get; set; }
        public DepartmentEntity()
        {
            Groups = new List<GroupEntity>();
        }
        public static bool operator==(DepartmentEntity d1, DepartmentEntity d2)
        {
            return d1.Id == d2.Id;
        }
        public static bool operator !=(DepartmentEntity d1, DepartmentEntity d2)
        {
            return d1.Id != d2.Id;
        }
    }
}