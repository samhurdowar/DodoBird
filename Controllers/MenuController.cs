
using Newtonsoft.Json;
using DodoBird.Models.Db;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DodoBird.Models;
using DodoBird.Services;
using System.Text;
using System.Data.SqlClient;

namespace DodoBird.Controllers
{
	public class MenuController : Controller
	{
        [HttpPost]
        public string GetAdminMenuList()
        {
            var clientResponse = HelperService.GetJsonData(1, "SELECT * FROM Menu ORDER BY ParentId, SortOrder");
            var json = JsonConvert.SerializeObject(clientResponse);
            return json;

        }

        [HttpPost]
        public string GetMenuList()
        {
            var clientResponse = HelperService.GetJsonData(1, "SELECT * FROM Menu ORDER BY ParentId, SortOrder");
            var json = JsonConvert.SerializeObject(clientResponse);
            return json;
        }

        [HttpPost]
        public string SortMenu(int menuId, int newOrder)
        {
            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.ExecuteSqlCommand("dbo.SortMenu @menuId, @newOrder", new[] { new SqlParameter("@menuId", menuId), new SqlParameter("@newOrder", newOrder) });
                    return "";
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public string GetMenuOptions()
        {
            var responseGrids = HelperService.GetJsonData(0, "SELECT GridId AS OptionValue, TableName  + ' - ' + GridName AS OptionText FROM Grid ORDER BY TableName, GridName");
            var responseForms = HelperService.GetJsonData(0, "SELECT FormId AS OptionValue, TableName  + ' - ' + FormName AS OptionText FROM Form ORDER BY TableName, FormName");

            return "{ \"Grids\" : " + responseGrids.JsonData + ", \"Forms\" : " + responseForms.JsonData + " } ";
        }

    }
}