
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using DodoBird.Models.Db;

namespace DodoBird.Controllers
{
	[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
	public class HomeController : Controller
	{
        public ActionResult Index()
		{
			return View();
		}

        public ActionResult GetPage(string pageFile)
        {
            return PartialView(pageFile);   //"~/Views/Home/Test.cshtml"
        }


    }


}