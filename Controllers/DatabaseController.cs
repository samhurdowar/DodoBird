using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DodoBird.Models.Db;
using System.Collections.Generic;
using System.Data.SqlClient;
using DodoBird.Models.App;
using DodoBird.Services;
using System.Text;
using DodoBird.Models;

namespace DodoBird.Controllers
{
    public class DatabaseController : Controller
    {
        public string GetDatabaseList()
        {
            var json = HelperService.GetJsonData(0, "SELECT * FROM AppDatabase");
            return json;
        }

        public string GetTableList(int appDatabaseId)
        {
            var sql = @"SELECT DISTINCT TABLE_SCHEMA AS TableSchema, TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE ";

            var json = HelperService.GetJsonData(appDatabaseId, sql);
            return json;
        }


        public string GetTableOjects(int appDatabaseId, string tableName)
        {
            // get TableSchema
            TableSchema tableSchema = DataService.GetTableSchema(appDatabaseId, tableName);
            var jsonTableSchema = JsonConvert.SerializeObject(tableSchema, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });


            // set default grid and form
            DataService.SetDefaultGridAndForm(appDatabaseId, tableName);


            // get grids
            var sql = @"SELECT * FROM Grid WHERE AppDatabaseId = @AppDatabaseId AND TableName = @TableName";
            var jsonGrids = HelperService.GetJsonData(0, sql, new[] { new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName) });


            // get forms
            sql = @"SELECT * FROM Form WHERE AppDatabaseId = @AppDatabaseId AND TableName = @TableName";
            var jsonForms = HelperService.GetJsonData(0, sql, new[] { new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName) });

            return "{ \"TableSchema\" : " + jsonTableSchema + ", \"Grids\" : " + jsonGrids + ", \"Forms\" : " + jsonForms + " } ";

        }



        public string GetGridSchema(int gridId)
        {
            // get TableSchema
            GridSchema gridSchema = DataService.GetGridSchema(gridId);
            var json = JsonConvert.SerializeObject(gridSchema, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return json;
        }

        public string SortGridColumn(int gridId, string columnName, int fromIndex, int toIndex, int newOrder)
        {
            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.ExecuteSqlCommand("dbo.SortGridColumn @gridId, @columnName, @fromIndex, @toIndex, @newOrder", new[] { new SqlParameter("@gridId", gridId), new SqlParameter("@columnName", columnName), new SqlParameter("@fromIndex", fromIndex), new SqlParameter("@toIndex", toIndex), new SqlParameter("@newOrder", newOrder) });
                    return "";
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

    }
}
