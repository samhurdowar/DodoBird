
using DodoBird.Models.App;
using DodoBird.Models.Db;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;

namespace DodoBird.Services
{
	public static class SessionService
	{
		public static int ClientId
        {
            get {
                try
                {
                    return (int)HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name];
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }

            set { HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name] = value; }
        }

        public static string GetConnectionString(int appDatabaseId)
        {

            try
            {
                var connectionString = "";
                if (HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name + appDatabaseId] == null)
                {
                    if (appDatabaseId > 0)
                    {
                        using (DodoBirdEntities Db = new DodoBirdEntities())
                        {
                            Db.Database.Connection.ConnectionString = SessionService.DodoBirdConnectionString;
                            connectionString = Db.Database.SqlQuery<string>("SELECT ConnectionString FROM AppDatabase WHERE AppDatabaseId = " + appDatabaseId).FirstOrDefault();
                        }
                    }
                    else
                    {
                        connectionString = SessionService.DodoBirdConnectionString;
                    }

                    HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name + appDatabaseId] = connectionString;
                }

                connectionString = HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name + appDatabaseId].ToString();
                return connectionString;
            }
            catch (System.Exception)
            {
                return "";
            }
            
        }

        public static string DodoBirdConnectionString
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name] == null)
                    {
                        HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name] = ConfigurationManager.AppSettings["DodoBirdConnectionString"].ToString();
                    }

                    var connectionString = HttpContext.Current.Session["app." + MethodBase.GetCurrentMethod().Name].ToString();
                    return connectionString;
                }
                catch (System.Exception)
                {
                    return "";
                }
            }
        }







    }
}