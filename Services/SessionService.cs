
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


    }
}