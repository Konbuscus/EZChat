using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EZChat_App.Helpers
{
    public class forbiddenWords
    {
        List<string> forbiddenList = new List<string> { "Enculé", "Ta gueule" };
        string sendMessage = "Ta gueule j'ai pas envie de t'écouter";

        public string CompareWords(string sendMessage, List<string> forbiddenList)
        {
            var forbidden = new Dictionary<string, string>();
            foreach (string str in forbiddenList)
            {
                forbidden.Add(str, "***");
            }
             
            Regex regex = new Regex(@"\s+");
           
            string[] words = regex.Split(sendMessage);

            var newphrase = new StringBuilder();
            foreach (string word in words)
            {              
                newphrase.Append(forbidden.ContainsKey(word) ? forbidden[word] : word);
                sendMessage = newphrase.ToString();
            }

            return sendMessage;
        }
            



    }


       





}
