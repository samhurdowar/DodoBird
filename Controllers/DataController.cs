
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