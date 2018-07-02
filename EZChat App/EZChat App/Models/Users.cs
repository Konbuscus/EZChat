using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZChat_App.Models
{
    public class Users
    {
        public ObjectId _id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public string BirthDate { get; set; }
        //TODO:  Compte FB (à la toute fin si le temps)
        public string PictureProfile { get; set; }
        public string Email { get; set; }
        public string RegisterDate { get; set; }

        //Elements contenant plusieurs ID pour liaisons
        public BsonArray FriendsListId { get; set; }
        public BsonArray PendingFriendsRequest { get; set; }


        //Le bson array contiendra tous les Id des amis de l'utilisateur
        public bool ConnectionStatus { get; set; }
    }
}