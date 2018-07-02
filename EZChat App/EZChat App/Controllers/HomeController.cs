using EZChat_App.App_Start;
using EZChat_App.Models;
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

        /// <summary>
        /// Inscription de l'utilisateur
        /// </summary>
        /// <returns></returns>
        public ActionResult SignUp()
        {
            //Redirection vers formulaire d'inscription
            return View();

        }

        /// <summary>
        /// Connexion de l'utilisateur
        /// </summary>
        /// <returns></returns>
        public ActionResult SignIn()
        {
            //Redirection vers un formulaire de login
            return View();
        }

        public JsonResult LoggingAction(string username, string password)
        {
            //On vérifique que l'utilisateur existe dans la base
            //Si il existe et que les données sont correctes, on le redirige sur la page d'accueil avec le tchat (côté client JS)
            //Possibilité d'intéragir avec le tchat
            return Json(true, JsonRequestBehavior.AllowGet);

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
            DateTime now = DateTime.Now;
            //Prepare to Insert
            var ChatMessage = new ChatMessage()
            {
                message = message,
                dateTime = now.ToString(),
                userId = "", //Methode pour récupérer l'id de l'utilisateur en fonction de son pseudonyme
            };


            // PAIR PROGRAMMING NE PAS TOUCHER var filtered = Helpers.BannedWordsFilterHelper.ReplaceBadWords(message);
           // _dbContext.database.GetCollection<ChatMessage>("Users").Insert(ChatMessage);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        
    }
}