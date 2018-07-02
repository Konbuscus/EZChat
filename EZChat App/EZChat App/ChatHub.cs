using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace EZChat_App
{
    public class ChatHub : Hub
    {
        public void Send(string nickname, string message)
        {
            Clients.All.addNewMessageToPage(nickname, message);

        }
    }
}