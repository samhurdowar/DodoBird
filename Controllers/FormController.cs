
using Newtonsoft.Json;
using DodoBird.Models.App;
using DodoBird.Models.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DodoBird.Services;
using DodoBird.Models;

namespace DodoBird.Controllers
{
    public class FormController : Controller
    {
        [HttpPost]
        public string GetFormSchema(int formId)
        {

            FormSchema formSchema = DataService.GetFormSchema(formId);

            var json = HelperService.GetJsonData(formSchema.AppDatabaseId, "SELECT * FROM Form WHERE FormId = @FormId", new SqlParameter[] { new SqlParameter("@FormId", formId) } );
            return json;

        }
    }
}