using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Book_Project_Goss.Controllers
{
    public class CalcController : Controller
    {
        // GET: Calc
        public ActionResult Calculator(string s1, string s2)
        {

            return View();
        }

        public ActionResult Multiply(string s1, string s2)
        {
            int i1 = int.Parse(s1);
            int i2 = int.Parse(s2);

            return View(s1, s2);
        }
    }
}