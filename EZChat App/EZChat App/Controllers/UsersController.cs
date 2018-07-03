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
        public ActionResult Index(string username)
        {
            _dbContext = new MongoContext();
            object userId;
            bool isForDisplayPurpose = false;
            if (string.IsNullOrEmpty(username))
            {
                userId = Session["User"];
            }
            else
            {
                userId = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("UserName", username))._id;
                isForDisplayPurpose = true;
            }
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
            ViewData["CountriesList"] = _dbContext.database.GetCollection<Countries>("Countries").FindAll().ToList();
            ViewData["SpokenLang"] = _dbContext.database.GetCollection<Lang>("Lang").FindAll().ToList();
            ViewData["ForDisplayOnly"] = isForDisplayPurpose;
            return View(user);
        }


        public ActionResult DisplayListofUsers()
        {
            _dbContext = new MongoContext();
            //Récupération de tous les utilisateurs dispo en bases excepté l'utilisateur lui même
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));

            List<Users> usersList = _dbContext.database.GetCollection<Users>("users").FindAll().ToList();
            List<Users> finalUserList = new List<Users>();

            for(int i = 0; i < usersList.Count(); i++)
            {
                if(usersList[i].UserName != user.UserName && !user.FriendsListId.Contains(usersList[i]._id))
                {
                    finalUserList.Add(usersList[i]);
                }
            }
            ViewData["User"] = user;
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
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));

            if (user != null)
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
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
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
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
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

        /// <summary>
        /// Deleting friends
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult DeleteFriends(string userName)
        {
            _dbContext = new MongoContext();
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
            Users usertToReject = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("UserName", userName));

            //Retirer les ID des listes d'amis
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null,
                Update.Pull("FriendsListId", usertToReject._id));
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", usertToReject._id), null,
                Update.Pull("FriendsListId", user._id));

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Suppression du profil
        /// </summary>
        /// <returns></returns>
        public JsonResult RemoveProfile()
        {

            _dbContext = new MongoContext();
            var userId = Session["User"];

            //Suppresison de l'utilisateur
            _dbContext.database.GetCollection<Users>("users").Remove(Query.EQ("_id", (ObjectId)userId));

            //Vidange Session
            Session["User"] = null;

            //Redirection sur la page de connexion
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public ActionResult FriendList()
        {
            _dbContext = new MongoContext();
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
            List<Users> friendsList = _dbContext.database.GetCollection<Users>("users").FindAll().Where(p => p.FriendsListId.Contains(user._id)).ToList();
            List<Users> emptyFriendList = new List<Users>();
            return View(friendsList == null ? emptyFriendList : friendsList);
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
            var userId = Session["User"];
            Users UserToEdit = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
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