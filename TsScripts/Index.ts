/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />

var Menus; 
let PageNavigations: { MenuId: number, MenuTitle: string, GridId: number, CurrentPage: number, NumOfPages: number, RecordCount: number, PrimaryKey: string, OrderByColumn: string, SortDirection: string }[] = [];
var CloseTabFirst = false; var DisableFocus = false;

$(document).ready(function () {
    GetMenuList();
});



function MenuClick(menuId: number) {
    AppSpinner(true);
    let isRefresh = false;
    var tabName = "tab" + menuId.toString();

    // refresh, set focus if tab already exists
    $(".tab-item").each(function () {
        var thisTabName = $(this).attr("id");
        if (tabName == thisTabName) {
            isRefresh = true;
        }
    });


    // get menu object
    let menu_ = Menus.filter(w => w.MenuId == menuId);
    let menu = menu_[0];

    // route menu =>  AddTab(level_MenuId: string, menuTitle: string, content: string) 
    if (menu.TargetId > 0 && menu.TargetType == "grid") {
        var gridId = menu.TargetId;
        SetPageNavigation(menu.MenuId, menu.MenuTitle, gridId);
        GetGrid(menu.MenuId, true);
        
    } else if (1 < 0) {
        //var content = "Content for Page " + m[0].MenuTitle + " - " + level_MenuId;
        //AddTab(level_MenuId, m[0].MenuTitle, content);
    } else if (menu.PageFile.length > 0 && menu.TargetType == "page") {
        $.ajax({
            url: "./Home/GetPage",
            data: { pageFile: menu.PageFile },
            type: "POST",
            dataType: "text",
            success: function (response) {
                AddTab(menu.MenuId, menu.MenuTitle, isRefresh, response.replace("[xxx]", "xxx"));
            },
            complete: function (response) {
                AppSpinner(false);
            }

        });
    }

    AppSpinner(false);
}

function SetPageNavigation(menuId: number, menuTitle: string, gridId: number) {
    var objIndex = PageNavigations.findIndex(obj => obj.MenuId == menuId);
    if (objIndex == -1) {
        PageNavigations.push({ MenuId: menuId, MenuTitle: menuTitle, GridId: gridId, CurrentPage: 1, NumOfPages: 0, RecordCount: 0, PrimaryKey: "", OrderByColumn: "", SortDirection: "ASC" });
    }
}

function AddTab(menuId: number, menuTitle: string, isRefresh: boolean, content: string) {
    if (!isRefresh) {
        $("#MainTab").append("<li id='tab" + menuId + "' class='tab-item' onclick=\"PageFocus('" + menuId + "')\">" + menuTitle + "<span class='main-tab-close fa fa-times' onclick=\"CloseTab('" + menuId + "')\"> </span> </li>");
        $("#MainPageContent").append("<li id='page" + menuId + "' class='page-content'>" + content + "</li>");
    } else {
        $("#page" + menuId).html(content);
    }

    PageFocus(menuId);
}

function PageFocus(menuId: number) {
    if (DisableFocus) {
        DisableFocus = false;
        return;
    }

    console.log("PageFocus menuId=" + menuId);

    $(".tab-item").removeClass("tab-active").removeClass("main-tab-li-hover");
    $(".tab-item").addClass("main-tab-li-hover");
    $("#tab" + menuId).removeClass("main-tab-li-hover").addClass("tab-active");

    $(".page-content").hide();
    $("#page" + menuId).show();

    if (CloseTabFirst) {
        DisableFocus = true;
        CloseTabFirst = false;
    }
}


function CloseTab(menuId: number) {
    // set focus to previous tab
    var previousMenuLevelId = Number($("#tab" + menuId).prev().attr("id").replace("tab", ""));
    CloseTabFirst = true;
    PageFocus(previousMenuLevelId);

    // remove tab
    $("#tab" + menuId).remove();
    $("#page" + menuId).remove();
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

function GetSelect(targetId, data) {
    var obj = [];

    for (var i in data) {
        var row = data[i];

        obj.push("<option value=\"" + row.OptionValue + "\">" + row.OptionText + "</option>");
    }

    var str = obj.join("");
    $("#" + targetId).html(str);
}