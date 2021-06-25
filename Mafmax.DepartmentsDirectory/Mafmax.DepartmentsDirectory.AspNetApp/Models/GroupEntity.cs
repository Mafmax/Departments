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
    [Table("Groups")]
    [Serializable]
    public class GroupEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public DepartmentEntity Department { get; set; }
        public GroupEntity()
        {

        }
    }
}