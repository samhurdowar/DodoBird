
using Newtonsoft.Json;
using DodoBird.Models.Db;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DodoBird.Controllers
{
	public class MenuController : Controller
	{

        public string GetMenuList()
        {
            using (DodoBirdEntities Db = new DodoBirdEntities())
            {
                var recs = Db.Menu1Item.Include(nameof(Menu1Item.Menu2Item)).ToList();
                var json = JsonConvert.SerializeObject(recs, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return json;
            }
        }

	}
}