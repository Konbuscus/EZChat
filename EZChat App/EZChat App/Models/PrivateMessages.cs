using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZChat_App.Models
{
    public class PrivateMessages
    {
        public ObjectId object_id { get; set; }
        public string message { get; set; }
        public BsonArray UsersIds { get; set; }
        public string dateTime { get; set; }
    }
}