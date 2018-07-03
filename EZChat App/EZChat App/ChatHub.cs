using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using EZChat_App.Models;
using System.Threading.Tasks;

namespace EZChat_App
{
    public class ChatHub : Hub
    {
        public void Send(string date, string nickname, string message)
        {
            var finalMessage = EZChat_App.Helpers.BannedWordsFilterHelper.ReplaceBadWords(message);
            Clients.All.addNewMessageToPage(date, nickname, finalMessage);

        }
    }
}