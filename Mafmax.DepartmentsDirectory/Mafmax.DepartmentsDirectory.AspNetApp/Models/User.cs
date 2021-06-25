using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mafmax.DepartmentsDirectory.AspNetApp.Models
{
    public class User
    {
        public string ConnectionId { get; set; }
        public UserSessionState SessionState { get; set; }
    }
}