//function ManageDatabase(level_MenuId: string, menuTitle: string) {
//    $.ajax({
//        url: "./Home/GetPage",
//        data: { pageFile: "~/Views/Home/ManageDatabase.cshtml" },
//        type: "POST",
//        dataType: "text",
//        success: function (response) {
//            response = response.replace("xxx", "You are the greatest");
//            AddTab(level_MenuId, menuTitle, response);
//        }
//    });
//}
function GetAppTable() {
    $.ajax({
        url: "./Entity/GetAppTable",
        type: "GET",
        dataType: "json",
        success: function (data) {
        }
    });
}
function GetDatabaseList() {
    $.ajax({
        url: "./Database/GetDatabaseList",
        dataType: "json",
        success: function (records) {
            var obj = [];
            obj.push("<table style='width:100%;'>");
            // record rows
            for (var i = 0; i < records.length; i++) {
                var row = records[i];
                obj.push("<tr onclick='SelectDatabase(" + row.AppDatabaseId + ")'>");
                obj.push("<td class='command-bar-select1'>" + row.DatabaseName + "</td>");
                obj.push("</tr>");
            }
            obj.push("</table>");
            $("#divAppDatabases").html(obj.join(""));
            SetCommandBarDOM();
        }
    });
}
function SelectDatabase(appDatabaseId) {
    $.ajax({
        url: "./Database/GetTableList",
        data: { appDatabaseId: appDatabaseId, includeColumns: false },
        dataType: "json",
        success: function (records) {
            var obj = [];
            obj.push("<table style='width:100%;'>");
            // record rows - tables
            for (var i = 0; i < records.length; i++) {
                var row = records[i];
                obj.push("<tr onclick='SelectTable(" + row.AppTableId + ")'>");
                obj.push("<td class='command-bar-select2'>" + row.TableName + "</td>");
                obj.push("</tr>");
            }
            obj.push("</table>");
            $("#divAppTables").html(obj.join(""));
            SetCommandBarDOM();
            // clear divAppColumns
            $("#divAppColumns").html("");
        }
    });
}
function SelectTable(appTableId) {
    $.ajax({
        url: "./Database/GetTableOjects",
        data: { appTableId: appTableId },
        dataType: "json",
        success: function (data) {
            // AppColumns
            var appColumns = data.AppColumns;
            var objAppColumns = [];
            objAppColumns.push("<table style='width:100%;'>");
            for (var i = 0; i < appColumns.length; i++) {
                var row = appColumns[i];
                objAppColumns.push("<tr id='AppColumnId" + row.AppColumnId + "' onclick='SelectColumn(this)'>");
                objAppColumns.push("<td class='command-bar-select3'>" + row.ColumnName + "</td>");
                objAppColumns.push("</tr>");
            }
            objAppColumns.push("</table>");
            $("#divAppColumns").html(objAppColumns.join(""));
            // Grids
            var grids = data.Grids;
            var objGrids = [];
            objGrids.push("<table style='width:100%;'>");
            for (var i = 0; i < grids.length; i++) {
                var row = grids[i];
                objGrids.push("<tr onclick='SelectGrid(" + row.GridId + ")'>");
                objGrids.push("<td class='command-bar-select3'>" + row.GridName + "</td>");
                objGrids.push("</tr>");
            }
            objGrids.push("</table>");
            $("#divGrids").html(objGrids.join(""));
            SetCommandBarDOM();
        }
    });
}
var to_isDisplayed = "";
function SelectGrid(gridId) {
    $.ajax({
        url: "./Database/GetGridColumnList",
        data: { gridId: gridId },
        dataType: "json",
        success: function (records) {
            var gridColumns = records;
            // GridColumns
            var objAvail = [];
            var objDisplayed = [];
            // available columns for grid
            var availColumns = gridColumns.filter(function (f) { return !f.IsDisplayed; }).sort(function (s) { return s.DisplayName; });
            objAvail.push("<ul id='availColumn' isDisplayed='0' class='connectedSortable'>");
            for (var i = 0; i < availColumns.length; i++) {
                var row = availColumns[i];
                objAvail.push("<li id='GridColumnId" + row.GridColumnId + "' isDisplayed='0' style='cursor:pointer;'>");
                objAvail.push("<span>" + row.AppColumn.DisplayName + "</span>");
                objAvail.push("</li>");
            }
            objAvail.push("</ul>");
            // columns displayed
            var displayColumns = gridColumns.filter(function (f) { return f.IsDisplayed; });
            displayColumns = SortJson(displayColumns, "SortOrder");
            objDisplayed.push("<ul id='displayColumn' isDisplayed='1' class='connectedSortable'>");
            for (var i = 0; i < displayColumns.length; i++) {
                var row = displayColumns[i];
                objDisplayed.push("<li id='GridColumnId" + row.GridColumnId + "' isDisplayed='1' style='cursor:pointer;'>");
                objDisplayed.push("<span>" + row.AppColumn.DisplayName + "</span>");
                objDisplayed.push("</li>");
            }
            objDisplayed.push("</ul>");
            // table render
            var obj = [];
            obj.push("<table>");
            obj.push("<tr>");
            obj.push("<td style='background: #ccc;width:300px;'>Available Fields</td>");
            obj.push("<td style='background: #ccc;width:300px;'>Display in Grid</td>");
            obj.push("</tr>");
            obj.push("<tr valign='top'>");
            obj.push("<td style='border:1px solid #ccc;width:300px;'>" + objAvail.join("") + "</td>");
            obj.push("<td style='border:1px solid #ccc;width:300px;'>" + objDisplayed.join("") + "</td>");
            obj.push("</tr>");
            obj.push("</table >");
            $("#divProperties").html(obj.join(""));
            // sortable
            $("#availColumn, #displayColumn").sortable({
                connectWith: ".connectedSortable",
                start: function (event, ui) {
                    ui.item.startPos = ui.item.index();
                    ui.placeholder.height(ui.item.height());
                },
                stop: function (event, ui) {
                    var gridColumnId = $(ui.item).attr("id").replace("GridColumnId", "");
                    var from_isDisplayed = $(ui.item).attr("isDisplayed");
                    var newOrder = ui.item.index() + 1;
                    if (to_isDisplayed == "")
                        to_isDisplayed = from_isDisplayed;
                    console.log("gridColumnId=" + gridColumnId + "    from_isDisplayed=" + from_isDisplayed + "    to_isDisplayed=" + to_isDisplayed + "    newOrder=" + newOrder);
                    SortGridColumn(gridId, gridColumnId, to_isDisplayed, newOrder);
                    if (from_isDisplayed != to_isDisplayed) {
                        $(ui.item).attr("isDisplayed", to_isDisplayed);
                    }
                    to_isDisplayed = "";
                },
                receive: function (event, ui) {
                    try {
                        to_isDisplayed = $(event.target).attr("isDisplayed");
                    }
                    catch (e) {
                        to_isDisplayed = "";
                    }
                }
            });
        }
    });
}
function SortGridColumn(gridId, gridColumnId, isDisplayed, newOrder) {
    $.ajax({
        url: "./Database/SortGridColumn",
        data: { gridId: gridId, gridColumnId: gridColumnId, isDisplayed: isDisplayed, newOrder: newOrder },
        dataType: "text",
        success: function (response) {
        }
    });
}
//# sourceMappingURL=ManageDatabase.js.map