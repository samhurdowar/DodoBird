//return "{ \"PrimaryKey\" : \"" + primaryKey + "\", \"RecordCount\" : " + recordCount + ", \"NumOfPages\" : " + numOfPages + ", \"OrderByColumn\" : \"" + pageNavigation.OrderByColumn + "\", \"SortDirection\" : \"" + pageNavigation.SortDirection + "\", \"Records\" : " + sb.ToString() + " }";
function GetGrid(level_MenuId, newTab) {
    var pageNavigation = PageNavigations.filter(function (it) { return it.MenuLevelId == level_MenuId; });
    $.ajax({
        url: "./Grid/GetGrid",
        data: pageNavigation[0],
        type: "POST",
        dataType: "json",
        success: function (data) {
            // update pageNavigation
            var i = PageNavigations.findIndex(function (obj) { return obj.MenuLevelId == level_MenuId; });
            if (i > -1) {
                PageNavigations[i].PrimaryKey = data.PrimaryKey;
                PageNavigations[i].RecordCount = data.RecordCount;
                PageNavigations[i].NumOfPages = data.NumOfPages;
                PageNavigations[i].OrderByColumn = data.OrderByColumn;
                PageNavigations[i].SortDirection = data.SortDirection;
            }
            GenerateGridTable(level_MenuId, newTab, data.Records);
        }
    });
}
function GenerateGridTable(level_MenuId, newTab, records) {
    var pageIndex = PageNavigations.findIndex(function (w) { return w.MenuLevelId == level_MenuId; });
    var primaryKey = PageNavigations[pageIndex].PrimaryKey;
    var menuTitle = PageNavigations[pageIndex].MenuTitle;
    var orderByColumn = PageNavigations[pageIndex].OrderByColumn;
    var sortDirection = PageNavigations[pageIndex].SortDirection;
    //var records = JSON.parse(jsonRecords);
    var obj = [];
    obj.push("<table style='width:100%;'>");
    // header row
    obj.push("<tr id='headerRow'>");
    var rowHeader = records[0];
    for (var column in rowHeader) {
        if (orderByColumn == column) {
            if (sortDirection == "ASC") {
                obj.push("<td>" + column + " <span class='fa fa-sort-down cursor-pointer' onclick='ChangeSortDirection()'> </span></td>");
            }
            else {
                obj.push("<td>" + column + " <span class='fa fa-sort-up cursor-pointer' onclick='ChangeSortDirection()'> </span></td>");
            }
        }
        else {
            obj.push("<td>" + column + " <span class='fa fa-sort sort-grey cursor-pointer' onclick=\"ChangeSortColumn('" + column + "')\"> </span></td>");
        }
    }
    obj.push("</tr>");
    // record rows
    for (var i = 0; i < records.length; i++) {
        var row = records[i];
        obj.push("<tr class='tr-all tr-row-pointer' id='row" + row[primaryKey] + "'>");
        for (var column in row) {
            obj.push("<td>" + row[column] + "</td>");
        }
        obj.push("</tr>");
    }
    obj.push("</table>");
    // page nav
    var pageDropdown = [];
    pageDropdown.push("<select id='pageDropdown" + level_MenuId + "' onChange=\"NavigatePage('" + level_MenuId + "',-111)\">");
    for (var p = 1; p <= PageNavigations[pageIndex].NumOfPages; p++) {
        pageDropdown.push("<option value='" + p + "'>" + p + "</option>");
    }
    pageDropdown.push("</select>");
    var numOfRecordsOnPage = 15;
    var toIndex = PageNavigations[pageIndex].CurrentPage * numOfRecordsOnPage;
    var fromIndex = toIndex - (numOfRecordsOnPage - 1);
    if (toIndex > PageNavigations[pageIndex].RecordCount) {
        toIndex = PageNavigations[pageIndex].RecordCount;
    }
    var recCountInfo = "Showing " + fromIndex + " to " + toIndex + " of " + PageNavigations[pageIndex].RecordCount + " records";
    obj.push("<div id='LookupNavBar' style='margin-bottom:20px;'>");
    obj.push("    <center>");
    obj.push("        <table style='width:93%;'>");
    obj.push("            <tr>");
    obj.push("                <td>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-double-left' style='cursor:pointer;' onclick=\"NavigatePage('" + level_MenuId + "',-100000)\"> </span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-left' style='cursor:pointer;' onclick=\"NavigatePage('" + level_MenuId + "',-1)\"> </span>");
    obj.push("                    <span id='navPages'>" + pageDropdown.join("") + "</span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-right' style='cursor:pointer;' onclick=\"NavigatePage('" + level_MenuId + "',1)\"> </span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-double-right' style='cursor:pointer;' onclick=\"NavigatePage('" + level_MenuId + "',100000)\"> </span>");
    obj.push("                </td> ");
    obj.push("                <td align='right'>" + recCountInfo + "</td>");
    obj.push("            </tr>");
    obj.push("        </table>");
    obj.push("    </center>");
    obj.push("</div>");
    obj.push();
    var content = obj.join("");
    if (newTab) {
        var content_ = "<div id='grid" + level_MenuId + "' style='margin-bottom:20px;'> " + content + "</div>";
        //xxxAddTab(level_MenuId, menuTitle, content_);
    }
    else {
        $("#grid" + level_MenuId).html(content);
    }
    // set pageDropdown
    $("#pageDropdown" + level_MenuId).val(PageNavigations[pageIndex].CurrentPage);
}
function NavigatePage(level_MenuId, pageIndex) {
    // update pageNavigation
    var i = PageNavigations.findIndex(function (obj) { return obj.MenuLevelId == level_MenuId; });
    var currentPage = 0;
    if (pageIndex == -111) {
        currentPage = Number($("#pageDropdown" + level_MenuId).val());
    }
    else {
        currentPage = PageNavigations[i].CurrentPage + pageIndex;
        if (currentPage < 1) {
            currentPage = 1;
        }
        else if (currentPage > PageNavigations[i].NumOfPages) {
            currentPage = PageNavigations[i].NumOfPages;
        }
    }
    PageNavigations[i].CurrentPage = currentPage;
    GetGrid(level_MenuId, false);
}
//function ChangeSortDirection() {
//    if (SortDirection == "ASC") {
//        SortDirection = "DESC";
//    } else {
//        SortDirection = "ASC";
//    }
//    GetLookups(1, CompanyId, CustomLabel, RefTable);
//}
//function ChangeSortColumn(columnName) {
//    OrderByColumn = columnName;
//    GetLookups(1, CompanyId, CustomLabel, RefTable);
//}
//# sourceMappingURL=Grid.js.map