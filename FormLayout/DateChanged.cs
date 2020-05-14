﻿using SourceControl.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SourceControl.FormLayoutClasses
{
	public partial class FormLayout
	{

		public string DateChanged(ColumnDef columnDef)
		{
			StringBuilder sb = new StringBuilder();

			var readOnly = "readonly";
			sb.AppendLine("<input type='text' id='P" + PageTemplateId + "_" + columnDef.ColumnName + "' name='P" + PageTemplateId + "_" + columnDef.ColumnName + "' style='width:" + columnDef.ElementWidth + ";' " + readOnly + " />");

			return sb.ToString();
		}

	}
}