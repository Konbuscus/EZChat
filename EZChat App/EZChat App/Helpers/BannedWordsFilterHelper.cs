
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace EZChat_App.Helpers
{
    public static  class BannedWordsFilterHelper
    {
        private static readonly HttpClient client = new HttpClient();
        //Yandex translation services
        const string APIKEY = "trnsl.1.1.20180628T122805Z.3ca63474aa6ba1cf.be62ffa0d9077cdc227814df665798343dc6914e";
        const string API = "https://translate.yandex.net/api/v1.5/tr.json";
        const string APIDETECT = API + "/detect?";
        const string APITRANSLATE = API + "/translate?key=" + APIKEY;

        public static async Task<string>  Translate(string text)
        {
            var values = new Dictionary<string, string>
            {
                { "key" ,APIKEY },
                {"text", text  }
            };

            JObject lang = new JObject();
            var content = new FormUrlEncodedContent(values);
            try
            {
                var tmp = client.PostAsync(APIDETECT, content).Result;
                if (tmp.IsSuccessStatusCode)
                {
                    var responseContent = tmp.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    lang= (JObject) JsonConvert.DeserializeObject(responseString);
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
                //On a la langue du mot
            }
            var translatingValues = new Dictionary<string, string>
            {
                {"text", text},
                {"lang", lang["lang"] + "-en"}
            };
            JObject resultTranslate = new JObject();
            var content2 = new FormUrlEncodedContent(translatingValues);
            var tmp2 =  client.PostAsync(APITRANSLATE, content2).Result;
            if (tmp2.IsSuccessStatusCode)
            {
                var responseContent2 = tmp2.Content;
                string response2string = responseContent2.ReadAsStringAsync().Result;
                resultTranslate = (JObject)JsonConvert.DeserializeObject(response2string);
            }
            return resultTranslate["text"][0].ToString();
        }



        public static string ReplaceBadWords(string message)
        {
            string[] cutMessage = message.Split(' ');
            List<string> TranslatedString = new List<string>();
            for(var i = 0; i < cutMessage.Length; i++)
            {
                var lul = Translate(cutMessage[i]);
                TranslatedString.Add(lul.Result);
            }

            //Pour chaque mot, on vérifie en quelle langue il est, 
            //Requête dans la base pour savoir si le mot est en anglais ou non
            //Si pas en anglais, requête vers google translate pour le traduire en anglais
            string strReplace = "";
            //Parcourir le fichier text pour trouver le mot.
            foreach(var line in File.ReadAllLines(@"D:\EZChat\ezchat\EZChat App\EZChat App\EZChat App\BannedWord.txt"))
            {
                foreach(var cut in TranslatedString)
                {
                    //Le mot est en anglais, et on va générer un nouveau mot
                    if(line == cut)
                    {
                        for(int i = 0; i <= line.Length; i++)
                        {
                            strReplace += "*";
                        }
                        //return strReplace;
                    }
                    
                }
            }
            return strReplace;
        }


    }
}