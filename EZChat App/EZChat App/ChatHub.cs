using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using EZChat_App.Models;
using System.Threading.Tasks;
using EZChat_App.App_Start;

namespace EZChat_App
{
    public class ChatHub : Hub
    {
        MongoContext _mongoContext;


        public void Send(string date, string nickname, string message)
        {
            _mongoContext = new MongoContext();
            var finalMessage = EZChat_App.Helpers.BannedWordsFilterHelper.ReplaceBadWords(message);
            var chatMessages = new ChatMessages()
            {
                username = nickname,
                dateTime = date,
                message = finalMessage
            };
            _mongoContext.database.GetCollection<ChatMessages>("ChatMessages").Insert<ChatMessages>(chatMessages);
            Clients.All.addNewMessageToPage(date, nickname, finalMessage);

        }
    }
}