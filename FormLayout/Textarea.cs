using SourceControl.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SourceControl.FormLayoutClasses
{
	public partial class FormLayout
	{

		public string Textarea(ColumnDef columnDef)
		{
			StringBuilder sb = new StringBuilder();
			string columnName = columnDef.ColumnName;

			int columnWidth = (columnDef.ElementWidth < 30) ? 300 : columnDef.ElementWidth;
			int columnHeight = (columnDef.ElementHeight < 10) ? 200 : columnDef.ElementHeight;

			sb.AppendLine("<textarea id='P" + PageTemplateId + "_" + columnDef.ColumnName + "' name='P" + PageTemplateId + "_" + columnDef.ColumnName + "' style='width:" + columnWidth + "px;height:" + columnHeight + "px;'></textarea>");

			return sb.ToString();
		}

	}
}