
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

namespace DodoBird.Controllers
{
	public class DataController : Controller
	{


        public string SaveFormData(string json)
        {


            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            var dodoBirdKey = jsonObj["DodoBirdKey"].ToString();
            var dodoBirdKey_ = dodoBirdKey.Split(new char[] { '|' });
            var appDatabaseId = Convert.ToInt32(dodoBirdKey_[0]);
            var tableName = dodoBirdKey_[1].ToString();

            var response = DataService.SaveFormData(appDatabaseId, tableName, jsonObj);


            return response;
        }



    }
}