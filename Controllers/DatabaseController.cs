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
using System;
using System.IO;

namespace DodoBird.Controllers
{
    public class DatabaseController : Controller
    {

        [HttpPost]
        public string InitDatabase(int appDatabaseId)
        {

            var filePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\InitDatabase.sql";

            var sql = System.IO.File.ReadAllText(filePath);

            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {

                    Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);

                    string[] exes = sql.Split(new string[] { "INTERNAL_GO" }, StringSplitOptions.None);
                    for (int i = 0; i < exes.Length; i++)
                    {
                        try
                        {
                            var exe = exes[i];

                            Db.Database.ExecuteSqlCommand(exe);
                        }
                        catch (Exception)
                        {
                        }

                    }

                    var clientResponse = new ClientResponse { Successful = true, ActionExecuted = "InitDatabase", JsonData = "", ErrorMessage = "" };
                    var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                    return jsonClientResponse;
                }
            }
            catch (Exception ex)
            {
                var clientResponse = new ClientResponse { Successful = false, ActionExecuted = "GetJsonData", JsonData = "", ErrorMessage = ex.Message };
                var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                return jsonClientResponse;
            }

        }


        [HttpPost]
        public string GetDatabaseList()
        {
            var clientResponse = HelperService.GetJsonData(0, "SELECT * FROM AppDatabase");
            var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);

            return jsonClientResponse;
        }

        [HttpPost]
        public string GetTableList(int appDatabaseId)
        {
            var sql = @"SELECT DISTINCT TABLE_SCHEMA AS TableSchema, TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE ";

            var clientResponse = HelperService.GetJsonData(appDatabaseId, sql);
            var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
            return jsonClientResponse;
        }

        [HttpPost]
        public string GetCustomOptionListxxx(int appDatabaseId)
        {
            try
            {
                var sql = @"SELECT * FROM _CustomOption WHERE AppDatabaseId = " + appDatabaseId;

                var clientResponse = HelperService.GetJsonData(appDatabaseId, sql);
                var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                return jsonClientResponse;
            }
            catch (Exception ex)
            {
                var clientResponse = new ClientResponse { Successful = false, ActionExecuted = "GetJsonData", JsonData = "", ErrorMessage = ex.Message };
                var jsonClientResponse = JsonConvert.SerializeObject(clientResponse);
                return jsonClientResponse;
            }

        }

        [HttpPost]
        public string GetTableOjects(int appDatabaseId, string tableName)
        {
            // get TableSchema
            TableSchema tableSchema = DataService.GetTableSchema(appDatabaseId, tableName);
            var jsonTableSchema = JsonConvert.SerializeObject(tableSchema, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });


            // set default grid and form
            DataService.SetDefaultGridAndForm(appDatabaseId, tableName);


            // get grids
            var sql = @"SELECT * FROM Grid WHERE AppDatabaseId = @AppDatabaseId AND TableName = @TableName";
            var responseGrids = HelperService.GetJsonData(0, sql, new[] { new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName) });
            var jsonGrids = responseGrids.JsonData;

            // get forms
            sql = @"SELECT * FROM Form WHERE AppDatabaseId = @AppDatabaseId AND TableName = @TableName";
            var responseForms = HelperService.GetJsonData(0, sql, new[] { new SqlParameter("@AppDatabaseId", appDatabaseId), new SqlParameter("@TableName", tableName) });
            var jsonForms = responseForms.JsonData;

            return "{ \"TableSchema\" : " + jsonTableSchema + ", \"Grids\" : " + jsonGrids + ", \"Forms\" : " + jsonForms + " } ";

        }


        [HttpPost]
        public string GetFormSchema(int formId)
        {
            FormSchema formSchema = DataService.GetFormSchema(formId);
            var json = JsonConvert.SerializeObject(formSchema, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return json;
        }


        [HttpPost]
        public string SortFormSection(int formId, int formSectionId, int newOrder)
        {
            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.ExecuteSqlCommand("dbo.SortFormSection @formId, @formSectionId, @newOrder", new[] { new SqlParameter("@formId", formId), new SqlParameter("@formSectionId", formSectionId), new SqlParameter("@newOrder", newOrder) });
                    return JsonConvert.SerializeObject(new ClientResponse { Successful = true, Id = formSectionId.ToString(), ActionExecuted = "SortFormSection", ErrorMessage = "" });
                }
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = formSectionId.ToString(), ActionExecuted = "SortFormSection", ErrorMessage = ex.Message });
            }
        }


        [HttpPost]
        public string SortFormColumn(int formId, string columnName, int toFormSectionId, int toSectionColumn, int newOrder)
        {
            try
            {
                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.ExecuteSqlCommand("dbo.SortFormColumn @formId, @columnName, @toFormSectionId, @toSectionColumn, @newOrder", new[] { new SqlParameter("@formId", formId), new SqlParameter("@columnName", columnName), new SqlParameter("@toFormSectionId", toFormSectionId), new SqlParameter("@toSectionColumn", toSectionColumn), new SqlParameter("@newOrder", newOrder) });
                    return JsonConvert.SerializeObject(new ClientResponse { Successful = true, Id = columnName, ActionExecuted = "SortFormColumn", ErrorMessage = "" });
                }
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = columnName, ActionExecuted = "SortFormColumn", ErrorMessage = ex.Message });
            }
        }



        [HttpPost]
        public string GetGridSchema(int gridId)
        {
            // get TableSchema
            GridSchema gridSchema = DataService.GetGridSchema(gridId);
            var json = JsonConvert.SerializeObject(gridSchema, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return json;
        }

        [HttpPost]
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



        [HttpPost]
        public string SaveTable(int appDatabaseId, string json)
        {
            try
            {
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                var sql = "";
                var oldTableName = jsonObj["OldTableName"].ToString();
                var tableName = jsonObj["TableName"].ToString();

                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);

                    if (oldTableName.Length > 0 && tableName.Length > 0)
                    {
                        if (oldTableName == "NEWTABLE")
                        {
                            sql = "CREATE TABLE " + jsonObj["TableName"] + "(" + jsonObj["TableName"] + "Id int not null primary key identity(1,1), AddDate datetime not null default getdate(), Modified datetime not null default getdate(), AddBy int not null default 0, ModifiedBy int not null default 0)";
                            Db.Database.ExecuteSqlCommand(sql);
                        }
                        else if (oldTableName != tableName)
                        {
                            TableSchema tableSchema = DataService.GetTableSchema(appDatabaseId, oldTableName);

                            sql = "sp_RENAME '" + tableSchema.Owner + "." + tableSchema.TableName + "' , '" + tableName + "'";
                            Db.Database.ExecuteSqlCommand(sql);
                        }
   
                    }

                }

                var response = JsonConvert.SerializeObject(new ClientResponse { Successful = true, Id = tableName, ActionExecuted = "SaveTable", JsonData = "", ErrorMessage = "" });

                return response;
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = appDatabaseId.ToString(), ActionExecuted = "SaveTable", ErrorMessage = ex.Message });
            }
        }



        //
        [HttpPost]
        public string DeleteTable(int appDatabaseId, string tableName)
        {
            try
            {
                var sql = "";

                using (DodoBirdEntities Db = new DodoBirdEntities())
                {
                    Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);

                    TableSchema tableSchema = DataService.GetTableSchema(appDatabaseId, tableName);

                    sql = "DROP TABLE " + tableSchema.Owner + "." + tableSchema.TableName;
                    Db.Database.ExecuteSqlCommand(sql);

                }

                var response = JsonConvert.SerializeObject(new ClientResponse { Successful = true, Id = tableName, ActionExecuted = "DeleteTable", JsonData = "", ErrorMessage = "" });

                return response;
            }
            catch (System.Exception ex)
            {
                return JsonConvert.SerializeObject(new ClientResponse { Successful = false, Id = appDatabaseId.ToString(), ActionExecuted = "DeleteTable", ErrorMessage = ex.Message });
            }
        }
    }
}
