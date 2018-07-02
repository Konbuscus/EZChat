using EZChat_App.App_Start;
using EZChat_App.Models;
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
            return View();
        }

        public JsonResult InsertMessagesInNoSQL(string nickName, string message)
        {
            _dbContext = new MongoContext();
            DateTime now = DateTime.Now;
            //Prepare to Insert
            Users user = Session["User"] as Users;
            if(user == null)
            {
                user = new Users()
                {
                    UserName = "Anonymous"
                    //Si anonyme on enregistre rien
                };
            }

            var ChatMessage = new ChatMessages()
            {
                message = message,
                dateTime = now.ToString(),
                userId = user._id
            };
            //Insertion en base du chat
            _dbContext.database.GetCollection<ChatMessages>("ChatMessages").Insert(ChatMessage);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Permet la déconnexion
        /// </summary>
        /// <returns></returns>
        public ActionResult Disconnect()
        {
            Users user = Session["User"] as Users;
            Session["User"] = null;
            _dbContext = new MongoContext();

            //Mis à jour du statut
            _dbContext.database.GetCollection<Users>("users").FindAndModify(Query.EQ("_id", user._id), null, Update.Set("ConnectionStatus", false));

            return RedirectToAction("Connect", "Signup");
        }


        
    }
}