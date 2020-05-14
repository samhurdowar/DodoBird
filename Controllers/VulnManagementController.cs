using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using SourceControl.Models.Db;
using SourceControl.Models;
using SourceControl.Services;

namespace SourceControl.Controllers
{
	[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
	public class VulnManagementController : Controller
	{
		public string NMSummary()
		{

			using (TargetEntities _db = new TargetEntities("VulnerabilityManagementEntities"))
			{
				SummaryData summaryData = new SummaryData();
				List<Notifications> criticalNotificationList = new List<Notifications>();
				summaryData.TotalRecordCount = _db.Database.SqlQuery<int>("Select Count(*) from NessusVulnData").SingleOrDefault();
				summaryData.DistinctPluginIDCount = _db.Database.SqlQuery<int>("Select Count(Distinct pluginID) from NessusVulnData").SingleOrDefault();
				summaryData.DistinctIPCount = _db.Database.SqlQuery<int>("Select Count(Distinct ip) from NessusVulnData").SingleOrDefault();
				summaryData.TotalWithExploit = _db.Database.SqlQuery<int>("Select count (exploitAvailable) from NessusVulnData Where exploitAvailable = 'true'").SingleOrDefault();
				summaryData.TotalMitigated = _db.Database.SqlQuery<int>("Select count(hasBeenMitigated) from NessusVulnData where hasBeenMitigated = '1'").SingleOrDefault();
				summaryData.Sev4RecordCount = _db.Database.SqlQuery<int>("Select count(pluginID) from NessusVulnData where severity = '4'").SingleOrDefault();
				summaryData.Sev3RecordCount = _db.Database.SqlQuery<int>("Select count(pluginID) from NessusVulnData where severity = '3'").SingleOrDefault();
				summaryData.Sev2RecordCount = _db.Database.SqlQuery<int>("Select count(pluginID) from NessusVulnData where severity = '2'").SingleOrDefault();
				summaryData.Sev1RecordCount = _db.Database.SqlQuery<int>("Select count(pluginID) from NessusVulnData where severity = '1'").SingleOrDefault();
				List<PluginCount> pluginCount = _db.Database.SqlQuery<PluginCount>("Select TOP 20 PluginID, Count(1) AS CurrentCount from NessusVulnData Group by pluginID order by CurrentCount desc").ToList();
				foreach (var item in pluginCount)
				{
					item.Description = _db.Database.SqlQuery<string>(string.Format("Select synopsis AS Description from NessusVulnData where pluginID = '{0}'", item.PluginID)).FirstOrDefault();
					item.PluginName = _db.Database.SqlQuery<string>(string.Format("Select PluginName from NessusVulnData where pluginID = '{0}'", item.PluginID)).FirstOrDefault();
				}
				summaryData.Top20PluginHitters = pluginCount;
				DateTime previous30DaysDate = DateTime.Now.AddDays(-30);
				DateTime previous15DaysDate = DateTime.Now.AddDays(-15);
				DateTime previous7DaysDate = DateTime.Now.AddDays(-7);
				summaryData.LastSeen30DayCount = _db.Database.SqlQuery<int>(string.Format("Select count(guid) from NessusVulnData where lastSeen > '{0}'", previous30DaysDate)).SingleOrDefault();
				summaryData.LastSeen15DayCount = _db.Database.SqlQuery<int>(string.Format("Select count(guid) from NessusVulnData where lastSeen > '{0}'", previous15DaysDate)).SingleOrDefault();
				summaryData.LastSeen7DayCount = _db.Database.SqlQuery<int>(string.Format("Select count(guid) from NessusVulnData where lastSeen > '{0}'", previous7DaysDate)).SingleOrDefault();
				summaryData.FirstSeen30DayCount = _db.Database.SqlQuery<int>(string.Format("Select count(guid) from NessusVulnData where firstSeen > '{0}'", previous30DaysDate)).SingleOrDefault();
				summaryData.FirstSeen15DayCount = _db.Database.SqlQuery<int>(string.Format("Select count(guid) from NessusVulnData where firstSeen > '{0}'", previous15DaysDate)).SingleOrDefault();
				summaryData.FirstSeen7DayCount = _db.Database.SqlQuery<int>(string.Format("Select count(guid) from NessusVulnData where firstSeen > '{0}'", previous7DaysDate)).SingleOrDefault();

				criticalNotificationList = _db.Database.SqlQuery<Notifications>("Select * from Notifications").OrderByDescending(a => a.date).ToList();

				var data = new
				{
					criticalNotificationList = criticalNotificationList,
					summaryData = summaryData
				};

				var json = new JavaScriptSerializer().Serialize(data);
				return json;
			}
		}

	}
}
