using Mafmax.DepartmentsDirectory.AspNetApp.Models;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mafmax.DepartmentsDirectory.AspNetApp
{
    public class ChatHub : Hub
    {
        //public  void Send(string name, string message)
        //{
        //     Clients.All.addMessage(name, message + " s");
        //}
        //public void Connect(string userName)
        //{
        //    var id = Context.ConnectionId;
        //    if (!Users.Any(x => x.ConnectionId == id))
        //    {
        //        Users.Add(new User { ConnectionId = id, Name = userName });
        //        Clients.Caller.onConnected(id, userName, Users);
        //        //TODO: Попробовать Clients.Others.onNewUserConnected
        //        Clients.AllExcept(id).onNewUserConnected(id, userName);
        //    }
        //}

        //public override Task OnDisconnected(bool stopCalled)
        //{

        //        var id = Context.ConnectionId;
        //    var item = Users.FirstOrDefault(x => x.ConnectionId == id);

        //    if(item != null)
        //    {
        //        Users.Remove(item);
        //        Clients.All.onUserDisconnected(id, item.Name);
        //    }
        //    return base.OnDisconnected(stopCalled);
        //}

    }
}