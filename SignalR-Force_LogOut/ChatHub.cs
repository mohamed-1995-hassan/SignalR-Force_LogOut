using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using SignalR_Force_LogOut.Models;
using System.Collections.Concurrent;

namespace SignalR_Force_LogOut
{
    public class ChatHub : Hub
    {
        public static Dictionary<string,string> MyUsers = new Dictionary<string,string>();
        

        public ChatHub() { }

        public string GetConnectionId()
        {
            if (Context.GetHttpContext().User.Identity.IsAuthenticated)
            {
                string name = Context.GetHttpContext().User.Identity.Name;
                if (MyUsers.ContainsKey(name))
                {
                    MyUsers[name] = Context.ConnectionId;
                }
            }
            return Context.ConnectionId;
        }
    }
}
