using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Mafmax.DepartmentsDirectory.AspNetApp.Startup))]

namespace Mafmax.DepartmentsDirectory.AspNetApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Дополнительные сведения о настройке приложения см. на странице https://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
            
        }
    }
}
