using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZChat_App.Models
{
    public class ChatMessages
    {
        public ObjectId _id { get; set; }
        public string message { get; set; }
        public string dateTime { get; set; }
        public string username { get; set; }

    }
}