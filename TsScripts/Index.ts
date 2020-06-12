﻿/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />

let Menu1Items: { Menu1ItemId: number, MenuTitle: string, PageFile: string, TargetId: number, TargetType: string, SortOrder: number }[] = [];
let Menu2Items: { Menu2ItemId: number, MenuTitle: string, PageFile: string, TargetId: number, TargetType: string, SortOrder: number }[] = [];
let Menu3Items: { Menu3ItemId: number, MenuTitle: string, PageFile: string, TargetId: number, TargetType: string, SortOrder: number }[] = [];


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

    // route menu =>  AddTab(level_MenuId: string, menuTitle: string, content: string) 
    if (m[0].TargetId > 0 && m[0].TargetType == "grid") {
        SetPageNavigation(level_MenuId, m[0].MenuTitle, m[0].TargetId);
        GetGrid(level_MenuId, true);
    } else if (1 < 0) {
        //var content = "Content for Page " + m[0].MenuTitle + " - " + level_MenuId;
        //AddTab(level_MenuId, m[0].MenuTitle, content);
    } else if (m[0].PageFile.length > 0 && m[0].TargetType == "page") {
        $.ajax({
            url: "./Home/GetPage",
            data: { pageFile: m[0].PageFile },
            type: "POST",
            dataType: "text",
            success: function (response) {
                AddTab(level_MenuId, m[0].MenuTitle, response.replace("[level_MenuId]"));
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
        $("#MainPageContent").append("<li id='page" + level_MenuId + "' class='page-content'>" + content + "</li>");
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


function SetCommandBarDOM() {

    $(".command-bar-select1").click(function () {
        $(".command-bar-select1").removeClass("command-bar-active");
        $(this).addClass("command-bar-active");
    });
    $(".command-bar-select2").click(function () {
        $(".command-bar-select2").removeClass("command-bar-active");
        $(this).addClass("command-bar-active");
    });
    $(".command-bar-select3").click(function () {
        $(".command-bar-select3").removeClass("command-bar-active");
        $(this).addClass("command-bar-active");
    });
    $(".command-bar-select4").click(function () {
        $(".command-bar-select4").removeClass("command-bar-active");
        $(this).addClass("command-bar-active");
    });
    $(".command-bar-select5").click(function () {
        $(".command-bar-select5").removeClass("command-bar-active");
        $(this).addClass("command-bar-active");
    });
    $(".command-bar-select6").click(function () {
        $(".command-bar-select6").removeClass("command-bar-active");
        $(this).addClass("command-bar-active");
    });
}

function SortJson(array, key) {
    return array.sort(function (a, b) {
        var x = a[key]; var y = b[key];
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    });
}


function OpenModalWindow(windowId) {
    $("#overlay").css({ "display": "block" });
    $("#" + windowId).css({ "display": "block" });
}


function CloseModalWindow(windowId) {
    $("#overlay").css({ "display": "none" });
    $("#" + windowId).css({ "display": "none" });
}

function MessageBox(title, msg, autoClose) {
    $("#messageBoxTitle").html(title);
    $("#messageBoxText").html(msg);
    OpenMessageBox();

    if (autoClose) {
        setTimeout(function () { CloseMessageBox(); }, 1500)
    }
}

function OpenMessageBox() {
    $("#overlay").css({ "display": "block" });
    $("#MessageBox").css({ "display": "block" });
}


function CloseMessageBox() {
    $("#overlay").css({ "display": "none" });
    $("#MessageBox").css({ "display": "none" });
}


function AppSpinner(tf) {
    if (tf) {
        $("#overlay").css({ "display": "block" });
        $("#appSpinner").addClass("lds-spinner");
    } else {
        $("#overlay").css({ "display": "none" });
        $("#appSpinner").removeClass("lds-spinner");
    }
}