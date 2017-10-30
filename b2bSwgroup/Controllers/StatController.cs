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

            if(DateTime.Now.Hour <= 7)
            {
                Random rand = new Random();
                posits = rand.Next(0,5);
            }
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour<=14)
            {
                Random rand = new Random();
                posits = rand.Next(3, 10);
                Specs = rand.Next(2,6);
            }
            if (DateTime.Now.Hour > 14 && DateTime.Now.Hour <= 18)
            {
                Random rand = new Random();
                posits = rand.Next(10, 30);
                Specs = rand.Next(5,10);
            }
            if (DateTime.Now.Hour > 18 && DateTime.Now.Hour <= 23)
            {
                Random rand = new Random();
                posits = rand.Next(5, 20);
                Specs = rand.Next(1, 4);
            }

            string message = "За последний час найдено позиций: " + posits.ToString() + ", подготовлено спецификаций:" + Specs.ToString();

            return message;
        }
    }
}