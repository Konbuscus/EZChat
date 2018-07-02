using EZChat_App.App_Start;
using EZChat_App.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZChat_App.Controllers
{
    public class UsersController : Controller
    {
        MongoContext _dbContext;
        // GET: Modifier profile
        public ActionResult Index()
        {
            _dbContext = new MongoContext();
            var user = Session["User"];
            ViewData["CountriesList"] = _dbContext.database.GetCollection<Countries>("Countries").FindAll().ToList();
            ViewData["SpokenLang"] = _dbContext.database.GetCollection<Lang>("Lang").FindAll().ToList();
            return View(user);
        }


        public ActionResult DisplayListofUsers()
        {
            _dbContext = new MongoContext();
            //Récupération de tous les utilisateurs dispo en bases excepté l'utilisateur lui même
            Users user = Session["User"] as Users;
            List<Users> usersList = _dbContext.database.GetCollection<Users>("users").FindAll().ToList();
            List<Users> finalUserList = new List<Users>();

            for(int i = 0; i < usersList.Count(); i++)
            {
                if(usersList[i].UserName != user.UserName && !user.FriendsListId.Contains(usersList[i]._id))
                {
                    finalUserList.Add(usersList[i]);
                }
            }

            return View(finalUserList);

        }


        /// <summary>
        /// Send a friend request
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult SendFriendRequest(string userName)
        {
            _dbContext = new MongoContext();
            Users user = Session["User"] as Users;

            if(user != null)
            {
                Users userToAsk = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("UserName", userName));
                //userToAsk.PendingFriendsRequest.Add(user._id);
                //Ajout de l'id dans notre pending friend request et dans celle de l'utilisateur concernée
                _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", userToAsk._id), null,
                    Update.AddToSet("PendingFriendsRequest", user._id));

                _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null,
                    Update.AddToSet("PendingFriendsRequest", userToAsk._id));
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>
        /// Cancel les requêtes d'amis des deux côtés
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult CancelFriendRequest(string userName)
        {
            _dbContext = new MongoContext();
            Users user = Session["User"] as Users;
            Users userToAsk = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("UserName", userName));
            //user.PendingFriendsRequest.Remove(userToAsk._id);
            //userToAsk.PendingFriendsRequest.Remove(user._id);

            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null,
                Update.Pull("PendingFriendsRequest", userToAsk._id));

            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", userToAsk._id), null,
                Update.Pull("PendingFriendsRequest", user._id));

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Accept a friend and insert it.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult AcceptFriendRequest(string userName)
        {
            _dbContext = new MongoContext();
            Users user = Session["User"] as Users;
            Users userToAccept = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("UserName", userName));

            //Retirer les ID des pendings
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null,
                Update.Pull("PendingFriendsRequest", userToAccept._id));
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", userToAccept._id), null,
                Update.Pull("PendingFriendsRequest", user._id));


            //Ajouter les ID dans les friends list respectivent
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", userToAccept._id), null,
                   Update.AddToSet("FriendsListId", user._id));

            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null,
                   Update.AddToSet("FriendsListId", userToAccept._id));

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FriendList()
        {
            _dbContext = new MongoContext();
            Users user = Session["User"] as Users;
            List<Users> friendsList = _dbContext.database.GetCollection<Users>("users").FindAll().Where(p => p.FriendsListId.Contains(user._id)).ToList();

            return View(friendsList);
        }


        public ActionResult SearchFriends()
        {
            return View();
        }

        /// <summary>
        /// Mise à jour de l'utilisateur
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public JsonResult UpdateUser(Users user)
        {
            _dbContext = new MongoContext();
            Users UserToEdit = Session["User"] as Users;
            try
            {
                _dbContext.database.GetCollection<Users>("users").FindAndModify(
                                Query.EQ("_id", UserToEdit._id),
                                null,
                                Update.Set("UserName", user.UserName)
                                .Set("FirstName", user.FirstName)
                                .Set("LastName", user.LastName)
                                .Set("CountryCode", user.CountryCode)
                                .Set("LanguageCode", user.LanguageCode)
                                .Set("PictureProfile", user.PictureProfile == null ? "" : user.PictureProfile)
                                .Set("Email", user.Email)
                                .Set("BirthDate", user.BirthDate),
                                true);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                e.GetBaseException();
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}