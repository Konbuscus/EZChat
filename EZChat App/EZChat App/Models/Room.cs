using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZChat_App.Models
{
    public class Room
    {
        public ObjectId object_id { get; set; }
        public BsonArray UsersId { get; set; }
        public List<PrivateMessages> privateMessages { get; set; }
    }
}