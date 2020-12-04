
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using DodoBird.Models.Db;
using DodoBird.Services;

namespace DodoBird.Controllers
{
	public class HomeController : Controller
	{
        public ActionResult Index()
		{
			return View();
		}


        [HttpPost]
        public ActionResult GetPage(int id, string targetType, string pageFile = "")
        {
            if (id > 0 && targetType == "form")
            {
                var formSchema = DataService.GetFormSchema(id);
                return PartialView(formSchema.PageFile);
            }



            return PartialView(pageFile);   //"~/Views/Home/Test.cshtml"
        }


    }


}