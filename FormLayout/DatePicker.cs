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
		public string DatePicker(ColumnDef columnDef)
		{
			StringBuilder sb = new StringBuilder();

			int columnWidth = (columnDef.ElementWidth < 10) ? 150 : columnDef.ElementWidth;
			sb.AppendLine("<input id='P" + PageTemplateId + "_" + columnDef.ColumnName + "' name='P" + PageTemplateId + "_" + columnDef.ColumnName + "' style='width:" + columnWidth + "px;' />");

			if (columnDef.DatePickerOption == "Date")
			{
				DocumentReady.AppendLine("$('#" + columnDef.ColumnName + "').kendoDatePicker({");
			}
			else if (columnDef.DatePickerOption == "DateTime")
			{
				//DocumentReady.AppendLine("$('#" + columnDef.ColumnName + "').kendoDateTimePicker({ format: 'MMMM yyyy hh:mmtt'");
				DocumentReady.AppendLine("$('#" + columnDef.ColumnName + "').kendoDateTimePicker({");
			}
			else if (columnDef.DatePickerOption == "MonthYear")
			{
				DocumentReady.AppendLine("$('#" + columnDef.ColumnName + "').kendoDatePicker({");
				DocumentReady.AppendLine("start: 'year',");
				DocumentReady.AppendLine("depth: 'year',");
				DocumentReady.AppendLine("format: 'MMMM yyyy'");
			}
			else if (columnDef.DatePickerOption == "Year")
			{
				DocumentReady.AppendLine("$('#" + columnDef.ColumnName + "').kendoDatePicker({");
				DocumentReady.AppendLine("start: 'year',");
				DocumentReady.AppendLine("depth: 'year',");
				DocumentReady.AppendLine("format: 'yyyy'");
			}


			DocumentReady.AppendLine("});");
			return sb.ToString();
		}

	}
}