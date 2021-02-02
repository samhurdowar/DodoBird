var PageNavigations = [];
//return "{ \"PrimaryKey\" : \"" + primaryKey + "\", \"RecordCount\" : " + recordCount + ", \"NumOfPages\" : " + numOfPages + ", \"OrderByColumn\" : \"" + pageNavigation.OrderByColumn + "\", \"SortDirection\" : \"" + pageNavigation.SortDirection + "\", \"Records\" : " + sb.ToString() + " }";
function SetPageNavigation(menuId, menuTitle, gridId) {
    var objIndex = PageNavigations.findIndex(function (obj) { return obj.MenuId == menuId; });
    if (objIndex == -1) {
        PageNavigations.push({ MenuId: menuId, MenuTitle: menuTitle, GridId: gridId, CurrentPage: 1, NumOfPages: 0, RecordCount: 0, OrderByColumn: "", SortDirection: "ASC" });
    }
}
function GetGrid(menuId, newTab) {
    var pageNavigation = PageNavigations.find(function (it) { return it.MenuId == menuId; });
    $.ajax({
        url: "./Grid/GetGrid",
        data: pageNavigation,
        type: "POST",
        dataType: "json",
        success: function (data) {
            // update pageNavigation
            var i = PageNavigations.findIndex(function (obj) { return obj.MenuId == menuId; });
            if (i > -1) {
                PageNavigations[i].RecordCount = data.RecordCount;
                PageNavigations[i].NumOfPages = data.NumOfPages;
                PageNavigations[i].OrderByColumn = data.OrderByColumn;
                PageNavigations[i].SortDirection = data.SortDirection;
            }
            GenerateGridTable(menuId, newTab, data.ToFormId, data.Records);
            //if (1==1) {
            //    GenerateCustomGrid(menuId, newTab, data.Records);
            //} else {
            //    GenerateGridTable(menuId, newTab, data.Records);
            //}
        }
    });
}
function GenerateCustomGrid(menuId, newTab, toFormId, records) {
    var pageIndex = PageNavigations.findIndex(function (w) { return w.MenuId == menuId; });
    var gridId = PageNavigations[pageIndex].GridId;
    //xxx var primaryKey = PageNavigations[pageIndex].PrimaryKey;
    var menuTitle = PageNavigations[pageIndex].MenuTitle;
    var orderByColumn = PageNavigations[pageIndex].OrderByColumn;
    var sortDirection = PageNavigations[pageIndex].SortDirection;
    //var records = JSON.parse(jsonRecords);
    var obj = [];
    obj.push("<table class='table-bordered table-padding-md' style='width:100%;'>");
    // record rows
    var NumOfColumn = 3;
    var columnCount = 0;
    obj.push("<tr>");
    for (var i = 0; i < records.length; i++) {
        var row = records[i];
        if (columnCount == NumOfColumn) {
            obj.push("</tr>");
            obj.push("<tr>");
            columnCount = 0;
        }
        obj.push("<td>" + row.GridRecord + "</td>");
        columnCount++;
    }
    // finish the cells
    if (columnCount < NumOfColumn) {
        for (var i = columnCount; i < NumOfColumn; i++) {
            obj.push("<td>xxx</td>");
            obj.push("</tr>");
        }
    }
    obj.push("</table>");
    // page navigation
    var pageDropdown = [];
    pageDropdown.push("<select id='pageDropdown" + menuId + "' onChange=\"NavigatePage('" + menuId + "',-111)\">");
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
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-double-left' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',-100000)\"> </span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-left' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',-1)\"> </span>");
    obj.push("                    <span id='navPages'>" + pageDropdown.join("") + "</span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-right' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',1)\"> </span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-double-right' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',100000)\"> </span>");
    obj.push("                </td> ");
    obj.push("                <td align='right'>" + recCountInfo + "</td>");
    obj.push("            </tr>");
    obj.push("        </table>");
    obj.push("    </center>");
    obj.push("</div>");
    obj.push();
    var content = obj.join("");
    if (newTab) { // first time. open new tab
        var content_ = "<div id='grid" + menuId + "_" + gridId + "' style='margin-bottom:20px;'> " + content + "</div>";
        AddTab(menuId, menuTitle, content_);
    }
    else {
        $("#grid" + menuId + "_" + gridId).html(content);
    }
    // set pageDropdown
    $("#pageDropdown" + menuId).val(PageNavigations[pageIndex].CurrentPage);
}
function GenerateGridTable(menuId, newTab, toFormId, records) {
    var pageIndex = PageNavigations.findIndex(function (w) { return w.MenuId == menuId; });
    var gridId = PageNavigations[pageIndex].GridId;
    var menuTitle = PageNavigations[pageIndex].MenuTitle;
    var orderByColumn = PageNavigations[pageIndex].OrderByColumn;
    var sortDirection = PageNavigations[pageIndex].SortDirection;
    //var records = JSON.parse(jsonRecords);
    var obj = [];
    obj.push("<table style='width:100%;'>");
    // header row
    obj.push("<tr class='headerColumn" + menuId + "'>");
    var headerColumns = records[0];
    for (var column in headerColumns) {
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
        obj.push("<tr key='" + row["PrimaryKeys"] + "' class='edit-row-" + menuId + " tr-all tr-row-pointer'>");
        for (var column in row) {
            obj.push("<td>" + row[column] + "</td>");
        }
        obj.push("</tr>");
    }
    obj.push("</table>");
    // page navigation
    var pageDropdown = [];
    pageDropdown.push("<select id='pageDropdown" + menuId + "' onChange=\"NavigatePage('" + menuId + "',-111)\">");
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
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-double-left' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',-100000)\"> </span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-left' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',-1)\"> </span>");
    obj.push("                    <span id='navPages'>" + pageDropdown.join("") + "</span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-right' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',1)\"> </span>");
    obj.push("                    <span class='hover-me nav-arrows fa fa-angle-double-right' style='cursor:pointer;' onclick=\"NavigatePage('" + menuId + "',100000)\"> </span>");
    obj.push("                </td> ");
    obj.push("                <td align='right'>" + recCountInfo + "</td>");
    obj.push("            </tr>");
    obj.push("        </table>");
    obj.push("    </center>");
    obj.push("</div>");
    obj.push();
    var content = obj.join("");
    if (newTab) { // first time. open new tab
        var content_ = "<div id='grid" + menuId + "_" + gridId + "' style='margin-bottom:20px;'> " + content + "</div><div id='form" + menuId + "_" + gridId + "' style='display:none;margin-bottom:20px;'></div>";
        AddTab(menuId, menuTitle, content_);
    }
    else {
        TurnOffSpinner(); // Call before content, so it waits before
        $("#grid" + menuId + "_" + gridId).html(content + AddAlive());
    }
    // set pageDropdown
    $("#pageDropdown" + menuId).val(PageNavigations[pageIndex].CurrentPage);
    // hide primaryKeys column 
    $(".headerColumn" + menuId + " td:first").hide();
    $(".edit-row-" + menuId + " td:first").hide();
    // go to edit form 
    $(".edit-row-" + menuId + " td").click(function () {
        $("#grid" + menuId + "_" + gridId).hide();
        AppSpinner(true);
        var key = $(this).parent().attr("key");
        // wait for form to post to html
        RandomRefreshObject = "init";
        var myVar = setInterval(function () {
            console.log("Looking for RandomRefreshObject=" + RandomRefreshObject);
            if ($("#" + RandomRefreshObject).length) {
                clearInterval(myVar);
                console.log("Found RandomRefreshObject=" + RandomRefreshObject);
                $.ajax({
                    url: "./Form/GetFormData",
                    type: "POST",
                    data: { json: key },
                    dataType: "json",
                    success: function (clientResponse) {
                        if (clientResponse.Successful) {
                            console.log("JsonData=" + clientResponse.JsonData);
                            var data_ = JSON.parse(clientResponse.JsonData);
                            var data = data_[0];
                            BindForm("FormId" + toFormId, data);
                            // show form
                            $("#form" + menuId + "_" + gridId).show();
                            // set go back button
                            $("#cmd_Cancel_FormId" + toFormId).click(function () {
                                // refresh grid if changed 
                                if ($("#FormId" + toFormId + " #FormSaved").val() == "T") {
                                    GetGrid(menuId, false);
                                }
                                // show grid
                                $("#form" + menuId + "_" + gridId).hide();
                                $("#grid" + menuId + "_" + gridId).show();
                            });
                        }
                        else {
                            MessageBox("Error", clientResponse.ErrorMessage, false);
                        }
                    },
                    complete: function () {
                        AppSpinner(false);
                    }
                });
            }
        }, 300);
        GetFormLayout(toFormId, "form" + menuId + "_" + gridId);
    });
}
function NavigatePage(menuId, pageIndex) {
    // update pageNavigation
    var i = PageNavigations.findIndex(function (obj) { return obj.MenuId == menuId; });
    var currentPage = 0;
    if (pageIndex == -111) {
        currentPage = Number($("#pageDropdown" + menuId).val());
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
    GetGrid(menuId, false);
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