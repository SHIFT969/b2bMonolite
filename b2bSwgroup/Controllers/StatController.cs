using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace b2bSwgroup.Controllers
{
    public class StatController : Controller
    {
        // GET: Stat
        public string Info()
        {
            // DateTime.Now.Hour;
            int posits = 0;
            int Specs = 0;

            if (DateTime.Now.Hour <= 7)
            {
                Random rand = new Random();
                posits = (int)(DateTime.Now.Hour * 1.3);
            }
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour <= 14)
            {
                Random rand = new Random();
                posits = (int)(DateTime.Now.Hour * 1.3);
                Specs = (int)(DateTime.Now.Hour * 0.5);
            }
            if (DateTime.Now.Hour > 14 && DateTime.Now.Hour <= 18)
            {
                Random rand = new Random();
                posits = (int)(DateTime.Now.Hour * 1.2);
                Specs = (int)(DateTime.Now.Hour * 0.5);
            }
            if (DateTime.Now.Hour > 18 && DateTime.Now.Hour <= 23)
            {
                Random rand = new Random();
                posits = (int)(DateTime.Now.Hour * 0.5);
                Specs = (int)(DateTime.Now.Hour * 0.1);
            }



            string message = "За последний час найдено позиций: " + posits.ToString() + ", подготовлено спецификаций:" + Specs.ToString();

            return message;
        }
    }
}