using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace Telerik.Reporting.DynamicTable
{

	public class Table : Telerik.Reporting.Table
	{
		private double rowHeight;
		private int columnIndex;
		private UnitType unitType;

		/// <summary>
		/// Returns the Header TextBox by header value
		/// </summary>
		public Dictionary<string, TextBox> HeaderTextBoxes { get; private set; }

		/// <summary>
		/// Returns the Body TextBox by data field value
		/// </summary>
		public Dictionary<string, TextBox> BodyTextBoxes { get; private set; }

		/// <summary>
		/// Initialize the Table with all of the required values
		/// </summary>
		/// <param name="name"></param>
		/// <param name="unitType"></param>
		/// <param name="locationX"></param>
		/// <param name="locationY"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="rowHeight"></param>
		/// <param name="dataSource"></param>
		private Table(string name,
				  UnitType unitType,
				  double locationX,
				  double locationY,
				  double width,
				  double height,
				  double rowHeight,
				  DataSource dataSource)
			 : this(name, unitType, locationX, locationY, width, height, dataSource)
		{
			this.rowHeight = rowHeight;
			base.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(new Telerik.Reporting.Drawing.Unit(rowHeight, unitType)));
			var detailGroup = new Telerik.Reporting.TableGroup();
			detailGroup.Groupings.Add(new Telerik.Reporting.Grouping(""));
			detailGroup.Name = "DetailGroup";
			base.RowGroups.Add(detailGroup);
		}

		/// <summary>
		/// Initialize the Table without detail group
		/// </summary>
		/// <param name="name">Table name</param>
		/// <param name="unitType">Unit Type</param>
		/// <param name="locationX">Table location X</param>
		/// <param name="locationY">Table location Y</param>
		/// <param name="width">Table width</param>
		/// <param name="height">Table width</param>
		/// <param name="rowHeight">Row Height</param>
		/// <param name="dataSource">DataSource</param>
		private Table(string name,
			 UnitType unitType,
			 double locationX,
			 double locationY,
			 double width,
			 double height,
			 DataSource dataSource) : base()
		{

			this.unitType = unitType;
			this.columnIndex = 0;
			this.HeaderTextBoxes = new Dictionary<string, TextBox>();
			this.BodyTextBoxes = new Dictionary<string, TextBox>();

			base.Name = name;
			base.Location = new PointU(new Unit(locationX, unitType),
												 new Unit(locationY, unitType));
			base.Size = new SizeU(new Unit(width, unitType),
											new Unit(height, unitType));
			base.DataSource = dataSource;
		}


		public static Telerik.Reporting.DynamicTable.Table CreateTable(string name,
			 UnitType unitType,
			 double locationX,
			 double locationY,
			 double width,
			 double height,
			 double rowHeight,
			 DataSource dataSource)
		{
			return new Table(name, unitType, locationX, locationY, width, height, rowHeight, dataSource);
		}

		/// <summary>
		/// Add new table column
		/// </summary>
		/// <param name="headerValue">Header textbox value</param>
		/// <param name="field">Data field</param>
		/// <param name="width">Column width</param>
		public Table WithColumn(
			 string headerValue,
			 string field,
			 double width)
		{
			var tableGroup = new Telerik.Reporting.TableGroup();

			var bodyTextBox = new TextBox()
			{
				Size = new SizeU(new Unit(width, this.unitType), new Unit(this.rowHeight, this.unitType)),
				Name = "ColumnTextBox" + field,
				Value = String.Format("=Fields.{0}", field)
			};

			bodyTextBox.Style.BorderColor.Default = Color.Black;
			bodyTextBox.Style.BorderStyle.Default = BorderType.Solid;

			this.BodyTextBoxes.Add(field, bodyTextBox);

			var headerTextBox = new TextBox()
			{
				Size = new SizeU(new Unit(width, this.unitType), new Unit(this.rowHeight, this.unitType)),
				Name = "ColumnTextBox" + field,
				Value = headerValue
			};
			this.HeaderTextBoxes.Add(headerValue, headerTextBox);

			base.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(width, this.unitType)));
			base.Body.SetCellContent(0, columnIndex++, bodyTextBox);
			tableGroup.Name = "ColumnGroup" + field;
			tableGroup.ReportItem = headerTextBox;
			base.ColumnGroups.Add(tableGroup);
			base.Items.Add(bodyTextBox);
			base.Items.Add(headerTextBox);

			return this;
		}
	}
}
