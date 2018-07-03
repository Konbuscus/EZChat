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
    public class SignUpController : Controller
    {
        MongoContext _dbContext;
        // GET: SignUp
        public ActionResult Index()
        {
            _dbContext = new MongoContext();
            Users user = new Users();
            //Chargement des pays pour la dropdown
            List<Countries> countriesList = _dbContext.database.GetCollection<Countries>("Countries").FindAll().ToList();
            List<Lang> langList = _dbContext.database.GetCollection<Lang>("Lang").FindAll().ToList();
            ViewData["CountriesList"] = countriesList;
            ViewData["SpokenLang"] = langList;

            return View(user);
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public JsonResult Register(Users user)
        {
            try
            {
                //Intilalisation des données vides
                _dbContext = new MongoContext();
                user.FriendsListId = new MongoDB.Bson.BsonArray();
                user.PendingFriendsRequest = new MongoDB.Bson.BsonArray();
                user.PictureProfile = "";
                user.RegisterDate = DateTime.Now.ToString();

                _dbContext.database.GetCollection<Users>("users").Insert<Users>(user);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                e.GetBaseException();
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Connect()
        {

            return View();
        }

        public JsonResult Connexion(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _dbContext = new MongoContext();
                //Vérification pour savoir si l'utilisateur existe, si il existe on stocke en session.
                //+ Redirection vers le profile
                //Sinon message d'erreur + reste sur la page de connexion
                var query = Query.EQ("UserName", username);
                Users userConnect = _dbContext.database.GetCollection<Users>("users").FindOne(query);

                if(userConnect != null)
                {
                    if(userConnect.UserName == username && userConnect.Password == password)
                    {
                        _dbContext.database.GetCollection<Users>("users").Update(Query.EQ("_id", userConnect._id), Update.Set("ConnectionStatus", true));
                        Session["User"] = userConnect._id;
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //Pas d'utilisateur trouvée
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }



    }
}