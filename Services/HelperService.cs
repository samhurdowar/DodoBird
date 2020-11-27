using DodoBird.Models.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace DodoBird.Services
{
    public static class HelperService
    {

        public static string GetJsonData(string sql, int appDatabaseId = 0, SqlParameter[] sqlParameters = null )
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                if (appDatabaseId > 0)
                {
                    Db.Database.Connection.ConnectionString = SessionService.GetConnectionString(appDatabaseId);
                } else
                {
                    Db.Database.Connection.ConnectionString = SessionService.DodoBirdConnectionString;
                }

                StringBuilder sb = new StringBuilder();


                if (sqlParameters != null)
                {
                    var items = Db.Database.SqlQuery<string>(sql + " FOR JSON AUTO, INCLUDE_NULL_VALUES", sqlParameters).ToList();
                    foreach (var item in items)
                    {
                        sb.Append(item);
                    }
                } else
                {
                    var items = Db.Database.SqlQuery<string>(sql + " FOR JSON AUTO, INCLUDE_NULL_VALUES").ToList();
                    foreach (var item in items)
                    {
                        sb.Append(item);
                    }
                }

                return sb.ToString();
            }
        }

        public static string InjectDodoBirdKey(int appDatabaseId, string tableName, string json)
        {
            string json_ = json;
            string injectKey = "[{ \"DodoBirdKey\": \"" + appDatabaseId + "|" + tableName + "\", \"";
            json_ = json_.Replace("[{\"", injectKey);

            return json_;
        }


    }
}