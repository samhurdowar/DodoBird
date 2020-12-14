
using Newtonsoft.Json;
using DodoBird.Models.Db;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DodoBird.Models;
using DodoBird.Services;
using System.Text;
using System.Data.SqlClient;
using System;
using DodoBird.Models.App;
using System.Collections.Generic;

namespace DodoBird.Controllers
{
	public class DataController : Controller
	{

        [HttpPost]
        public string GetLookups()
        {
            //

            var forms = HelperService.GetJsonData(1, "SELECT * FROM Form");
            var dataTypes = HelperService.GetJsonData(1, "SELECT name AS DataType FROM sys.Types WHERE system_type_id = user_type_id ORDER BY name");

            return "{ \"Forms\" : " + forms.JsonData + ", \"DataTypes\" : " + dataTypes.JsonData + " } ";
        }

    }
}