
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


        public string GetMenuItem(int menuId)
        {
            var json = HelperService.GetJsonData("SELECT * FROM Menu WHERE MenuId = @MenuId", 0, new[] { new SqlParameter("@MenuId", menuId) });

            json = HelperService.InjectDodoBirdKey(0, "Menu", json);  
            return json;
        }

        public string GetAdminMenuList()
        {
            var json = HelperService.GetJsonData("SELECT * FROM Menu");
            return json;
        }


        public string GetMenuList()
        {
            var json = HelperService.GetJsonData("SELECT * FROM Menu");
            return json;
        }

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

        public string GetMenuOptions()
        {
            var jsonGrids = HelperService.GetJsonData("SELECT GridId AS OptionValue, TableName  + ' - ' + GridName AS OptionText FROM Grid ORDER BY TableName, GridName");
            var jsonForms = HelperService.GetJsonData("SELECT FormId AS OptionValue, TableName  + ' - ' + FormName AS OptionText FROM Form ORDER BY TableName, FormName");

            return "{ \"Grids\" : " + jsonGrids + ", \"Forms\" : " + jsonForms + " } ";
        }

    }
}