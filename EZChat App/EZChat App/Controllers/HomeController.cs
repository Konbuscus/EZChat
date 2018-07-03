using EZChat_App.App_Start;
using EZChat_App.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EZChat_App.Controllers
{
    public class HomeController : Controller
    {

        MongoContext _dbContext;

        public ActionResult Index()
        {
            //Page de base d'accueil 
            //L'utilisateur peut se connecter sur cette page et visualiser le chat sans pouvoir intéragir avec celui-ci
            //La vue doit afficher le chat.
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            _dbContext = new MongoContext();
            var userId = Session["User"];
            if(userId != null)
            {
                Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
                ViewData["User"] = user;

            }
            List<ChatMessages> chatMessageList = _dbContext.database.GetCollection<ChatMessages>("ChatMessages").FindAll().ToList();
            return View(chatMessageList);
        }

        public JsonResult InsertMessagesInNoSQL(string nickName, string message)
        {
            _dbContext = new MongoContext();
            DateTime now = DateTime.Now;
            //Prepare to Insert
            var userId = Session["User"];
            if(userId == null)
            {
                //Users users = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
                
                    Users users = new Users()
                    {
                        UserName = "Anonymous"
                        //Si anonyme on enregistre rien
                    };
                    return Json(true, JsonRequestBehavior.AllowGet);
                
            }
            else
            {
                Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("UserName", nickName));
                var ChatMessage = new ChatMessages()
                {
                    message = message,
                    dateTime = now.ToString(),
                    username = user.UserName
                };
                //Insertion en base du chat
                _dbContext.database.GetCollection<ChatMessages>("ChatMessages").Insert(ChatMessage);
            }
            
               
            
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Permet la déconnexion
        /// </summary>
        /// <returns></returns>
        public ActionResult Disconnect()
        {
            _dbContext = new MongoContext();
            var userId = Session["User"];
            Users user = _dbContext.database.GetCollection<Users>("users").FindOne(Query.EQ("_id", (ObjectId)userId));
            Session["User"] = null;
            

            //Mis à jour du statut
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null, Update.Set("ConnectionStatus", false));

            return RedirectToAction("Connect", "Signup");
        }


        
    }
}