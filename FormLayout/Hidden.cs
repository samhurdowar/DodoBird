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

		public string Hidden(ColumnDef columnDef)
		{
			return "<input type='hidden'  id='P" + PageTemplateId + "_" + columnDef.ColumnName + "' name='P" + PageTemplateId + "_" + columnDef.ColumnName + "' />";
		}

	}
}