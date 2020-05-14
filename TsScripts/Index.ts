/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />

let Menu1Items: { Menu1ItemId: number, MenuTitle: string, PageFile: string, TsScript: string, GridId: number, FormId: number, SortOrder: number }[] = [];
let Menu2Items: { Menu2ItemId: number, MenuTitle: string, PageFile: string, TsScript: string, GridId: number, FormId: number, SortOrder: number }[] = [];
let Menu3Items: { Menu3ItemId: number, MenuTitle: string, PageFile: string, TsScript: string, GridId: number, FormId: number, SortOrder: number }[] = [];

let PageNavigations: { MenuLevelId: string, MenuTitle: string, GridId: number, CurrentPage: number, NumOfPages: number, RecordCount: number, PrimaryKey: string, OrderByColumn: string, SortDirection: string }[] = [];
var CloseTabFirst = false; var DisableFocus = false;
var IsRefresh = false;

$(document).ready(function () {
    GetMenuList();
});


function RefreshPage(level_MenuId: string) {
    var ray = level_MenuId.split("_");
    var level = Number(ray[0]);
    var menuId = Number(ray[1]);
    MenuClick(level, menuId, true);
}

function MenuClick(level: number, id: number, isRefresh: boolean) {
    IsRefresh = isRefresh;
    var level_MenuId = level.toString() + "_" + id.toString();

    // set focus if tab already exists
    var tabExist = false;
    if (!IsRefresh) {
        $(".tab-item").each(function () {
            var tabId = $(this).attr("id");
            if (tabId == "tab" + level_MenuId) {
                PageFocus(level_MenuId);
                tabExist = true;
            }
        });
    }
    if (tabExist) return;

    // get menu object
    var evalString = "Menu" + level + "Items.filter(it => it.Menu" + level + "ItemId == id)";
    let m = eval(evalString);

    // route menu
    if (m[0].GridId > 0) {
        SetPageNavigation(level_MenuId, m[0].MenuTitle, m[0].GridId);
        GetGrid(level_MenuId, true);
    } else if (m[0].FormId > 0) {
        var content = "Content for Page " + m[0].MenuTitle + " - " + level_MenuId;
        AddTab(level_MenuId, m[0].MenuTitle, content);
    } else if (m[0].PageFile.length > 0) {
        $.ajax({
            url: "./Home/GetPage",
            data: { pageFile: m[0].PageFile },
            type: "POST",
            dataType: "text",
            success: function (response) {
                AddTab(level_MenuId, m[0].MenuTitle, response.replace("[level_MenuId]", level_MenuId));
            }
        });
    } else if (m[0].TsScript != null && m[0].TsScript.length > 0) {
        eval(m[0].TsScript.replace("()","(level_MenuId, m[0].MenuTitle)"));
    }
}

function SetPageNavigation(level_MenuId: string, menuTitle: string, gridId: number) {
    var objIndex = PageNavigations.findIndex(obj => obj.MenuLevelId == level_MenuId);
    if (objIndex == -1) {
        PageNavigations.push({ MenuLevelId: level_MenuId, MenuTitle: menuTitle, GridId: gridId, CurrentPage: 1, NumOfPages: 0, RecordCount: 0, PrimaryKey: "", OrderByColumn: "", SortDirection: "ASC" });
    }
}

function AddTab(level_MenuId: string, menuTitle: string, content: string) {
    if (!IsRefresh) {
        $("#MainTab").append("<li id='tab" + level_MenuId + "' class='tab-item' onclick=\"PageFocus('" + level_MenuId + "')\">" + menuTitle + "<span class='main-tab-refresh-close fa fa-times' onclick=\"CloseTab('" + level_MenuId + "')\"> </span> <span class='main-tab-refresh-close fa fa-redo' onclick=\"RefreshPage('" + level_MenuId + "')\"> </span></li>");
        $("#MainPage").append("<li id='page" + level_MenuId + "' class='page-content'>" + content + "</li>");
    } else {
        $("#page" + level_MenuId).html(content);
    }

    PageFocus(level_MenuId);
}

function PageFocus(level_MenuId: string) {
    if (DisableFocus) {
        DisableFocus = false;
        return;
    }

    console.log("PageFocus=" + level_MenuId);

    $(".tab-item").removeClass("tab-active").removeClass("main-tab-li-hover");
    $(".tab-item").addClass("main-tab-li-hover");
    $("#tab" + level_MenuId).removeClass("main-tab-li-hover").addClass("tab-active");

    $(".page-content").hide();
    $("#page" + level_MenuId).show();

    if (CloseTabFirst) {
        DisableFocus = true;
        CloseTabFirst = false;
    }
}


function CloseTab(level_MenuId: string) {
    // set focus to previous tab
    var previousMenuLevelId = $("#tab" + level_MenuId).prev().attr("id").replace("tab", "");
    CloseTabFirst = true;
    PageFocus(previousMenuLevelId);

    // remove tab
    $("#tab" + level_MenuId).remove();
    $("#page" + level_MenuId).remove();
}
