
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


        public string GetFormData(string json)
        {
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            var formId = Convert.ToInt32(jsonObj["FormId"]);

            FormSchema formSchema = DataService.GetFormSchema(formId);
            TableSchema tableSchema = DataService.GetTableSchema(formSchema.AppDatabaseId, formSchema.TableName);

            // generate sql statement
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            StringBuilder sb = new StringBuilder();
            sb.Append("FROM " + formSchema.TableName + " WHERE ");

            // primary keys
            var isNewRecord = false;
            foreach (var key in tableSchema.PrimaryKeys)
            {
                if (jsonObj[key.ColumnName] != null)
                {
                    var keyValue = jsonObj[key.ColumnName].ToString();
                    if (keyValue == "" || keyValue == "0")
                    {
                        isNewRecord = true;
                        break;
                    }

                    sb.Append(key.ColumnName + "= @" + key.ColumnName + " AND ");
                    
                    sqlParameters.Add(new SqlParameter("@" + key.ColumnName, keyValue));
                } else
                {
                    isNewRecord = true;
                }
            }

            if (isNewRecord)  // get empty template with defaults
            {
                var defaultValue = "";
                sb.Clear();
                sb.Append("[ { ");
                foreach (var column in tableSchema.Columns)
                {
                    defaultValue = column.DefaultValue;
                    if (defaultValue == "getdate")
                    {
                        defaultValue = DateTime.Now.ToString();
                    } 
                    sb.Append("\"" + column.ColumnName + "\":\"" + defaultValue + "\", ");
                }
                json = sb.ToString() + " \"IsNewRecord\": \"" + isNewRecord + "\" } ]";

            } else
            {
                var sql = sb.ToString();
                sql = "SELECT 'False' AS IsNewRecord, * " + sql.Substring(0, sql.Length - 4);

                json = HelperService.GetJsonData(formSchema.AppDatabaseId, sql, sqlParameters.ToArray());
            }


            return json;
        }

        public string SaveFormData(string json)
        {
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            var formId = Convert.ToInt32(jsonObj["FormId"]);
            var formSchema = DataService.GetFormSchema(formId);

            var response = DataService.SaveFormData(formSchema.AppDatabaseId, formSchema.TableName, json);

            return response;
        }

        public string DeleteData(string json)
        {

            try
            {
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var formId = Convert.ToInt32(jsonObj["FormId"]);
                var response = "";
                FormSchema formSchema = DataService.GetFormSchema(formId);
                TableSchema tableSchema = DataService.GetTableSchema(formSchema.AppDatabaseId, formSchema.TableName);

                // generate sql statement
                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM " + formSchema.TableName + " WHERE ");

                // primary keys
                var completeKeys = true;
                foreach (var key in tableSchema.PrimaryKeys)
                {
                    if (jsonObj[key.ColumnName] != null)
                    {
                        var keyValue = jsonObj[key.ColumnName].ToString();
                        if (keyValue == "" || keyValue == "0")
                        {
                            completeKeys = false;
                            break;
                        }

                        sb.Append(key.ColumnName + "= @" + key.ColumnName + " AND ");

                        sqlParameters.Add(new SqlParameter("@" + key.ColumnName, keyValue));
                    }
                    else
                    {
                        completeKeys = false;
                    }
                }

                if (completeKeys)  // get empty template with defaults
                {
                    var sql = sb.ToString();
                    sql = sql.Substring(0, sql.Length - 4);

                    response = HelperService.ExecuteSql(formSchema.AppDatabaseId, sql, sqlParameters.ToArray());

                }
                else
                {
                    return "Keys received were incomplete.";
                }

                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }



    }
}