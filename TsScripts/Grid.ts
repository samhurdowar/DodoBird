let PageNavigations: { MenuId: number, MenuTitle: string, GridId: number, CurrentPage: number, NumOfPages: number, RecordCount: number, PrimaryKey: string, OrderByColumn: string, SortDirection: string }[] = [];


//return "{ \"PrimaryKey\" : \"" + primaryKey + "\", \"RecordCount\" : " + recordCount + ", \"NumOfPages\" : " + numOfPages + ", \"OrderByColumn\" : \"" + pageNavigation.OrderByColumn + "\", \"SortDirection\" : \"" + pageNavigation.SortDirection + "\", \"Records\" : " + sb.ToString() + " }";


function SetPageNavigation(menuId: number, menuTitle: string, gridId: number) {
    var objIndex = PageNavigations.findIndex(obj => obj.MenuId == menuId);
    if (objIndex == -1) {
        PageNavigations.push({ MenuId: menuId, MenuTitle: menuTitle, GridId: gridId, CurrentPage: 1, NumOfPages: 0, RecordCount: 0, PrimaryKey: "", OrderByColumn: "", SortDirection: "ASC" });
    }
}

function GetGrid(menuId: number, newTab: boolean) {
    let pageNavigation = PageNavigations.find(it => it.MenuId == menuId);

    $.ajax({
        url: "./Grid/GetGrid",
        data: pageNavigation,
        type: "POST",
        dataType: "json",
        success: function (data) {

            // update pageNavigation
            var i = PageNavigations.findIndex(obj => obj.MenuId == menuId);
            if (i > -1) {
                //xxxPageNavigations[i].PrimaryKey = data.PrimaryKey;
                PageNavigations[i].RecordCount = data.RecordCount;
                PageNavigations[i].NumOfPages = data.NumOfPages;
                PageNavigations[i].OrderByColumn = data.OrderByColumn;
                PageNavigations[i].SortDirection = data.SortDirection;
            }


            GenerateGridTable(menuId, newTab, data.Records);

            //if (1==1) {
            //    GenerateCustomGrid(menuId, newTab, data.Records);
            //} else {
            //    GenerateGridTable(menuId, newTab, data.Records);
            //}
            
        },
        complete: function () {
            AppSpinner(false);
        }
    });
}



function GenerateCustomGrid(menuId: number, newTab: boolean, records) {

    var pageIndex = PageNavigations.findIndex(w => w.MenuId == menuId);
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

    if (newTab) {  // first time. open new tab
        var content_ = "<div id='grid" + menuId + "_" + gridId + "' style='margin-bottom:20px;'> " + content + "</div>";
        AddTab(menuId, menuTitle, false, content_);
    } else {
        $("#grid" + menuId + "_" + gridId).html(content);
    }

    // set pageDropdown
    $("#pageDropdown" + menuId).val(PageNavigations[pageIndex].CurrentPage);

}

function GenerateGridTable(menuId: number, newTab: boolean, records) {
    
    var pageIndex = PageNavigations.findIndex(w => w.MenuId == menuId);
    var gridId = PageNavigations[pageIndex].GridId;
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
            } else {
                obj.push("<td>" + column + " <span class='fa fa-sort-up cursor-pointer' onclick='ChangeSortDirection()'> </span></td>");
            }

        } else {
            obj.push("<td>" + column + " <span class='fa fa-sort sort-grey cursor-pointer' onclick=\"ChangeSortColumn('" + column + "')\"> </span></td>");
        }
    }
    obj.push("</tr>");

    // record rows
    for (var i = 0; i < records.length; i++) {
        var row = records[i];
        obj.push("<tr class='tr-all tr-row-pointer'>");  
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

    if (newTab) {  // first time. open new tab
        var content_ = "<div id='grid" + menuId + "_" + gridId + "' style='margin-bottom:20px;'> " + content + "</div>";
        AddTab(menuId, menuTitle, false, content_);
    } else {
        $("#grid" + menuId + "_" + gridId).html(content);
    }
    
    // set pageDropdown
    $("#pageDropdown" + menuId).val(PageNavigations[pageIndex].CurrentPage);

}

function NavigatePage(menuId: number, pageIndex: number) {

    // update pageNavigation
    var i = PageNavigations.findIndex(obj => obj.MenuId == menuId);

    var currentPage = 0;
    if (pageIndex == -111) {
        currentPage = Number($("#pageDropdown" + menuId).val());
    } else {
        currentPage = PageNavigations[i].CurrentPage + pageIndex;

        if (currentPage < 1) {
            currentPage = 1;
        } else if (currentPage > PageNavigations[i].NumOfPages) {
            currentPage = PageNavigations[i].NumOfPages;
        }
    }

    PageNavigations[i].CurrentPage = currentPage;
    GetGrid(menuId, false)
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
