using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EZChat_App.Models
{
    public class ProfilePicture
    {

        private GenericImage GenerateAvatarImage(String text, Font font, Color textColor, Color backColor, string filename)
        {

            //first, create a dummy bitmap just to get a graphics object  
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be  
            SizeF textSize = drawing.MeasureString(text, font);

            img.Dispose();
            drawing.Dispose(); 
            img = new Bitmap(110, 110);

            drawing = Graphics.FromImage(img);
            drawing.Clear(backColor);

            //create a brush for the text  
            Brush textBrush = new SolidBrush(textColor);

            //drawing.DrawString(text, font, textBrush, 0, 0);  
            drawing.DrawString(text, font, textBrush, new Rectangle(-2, 20, 200, 110));
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            img.Save(Server.MapPath("~/Images/" + filename + ".gif"));

            return img;

        }
  
        //    Font font = new Font(FontFamily.GenericSerif, 45, FontStyle.Bold);
        //    Color fontcolor = ColorTranslator.FromHtml("#FFF");
        //    Color bgcolor = ColorTranslator.FromHtml("#83B869");
        //    GenerateAvtarImage("LOL", font, fontcolor, bgcolor, "test");
    
        private CustomImage UploadAvatarImage()
        {
            DeleteFile(fileName);
            UploadAvatarImage(HttpPostedFileBaseFile);

            return img;
        }


    }
}