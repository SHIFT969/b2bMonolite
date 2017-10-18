using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace b2bSwgroup.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotOrganization()
        {
            return View();
        }
        public ActionResult NotOrganizationPartial()
        {
            return View();
        }
    }
}