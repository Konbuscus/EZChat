using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZChat_App.Models
{
    public class Lang
    {
        public ObjectId _id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string nativeName { get; set; }
    }
}