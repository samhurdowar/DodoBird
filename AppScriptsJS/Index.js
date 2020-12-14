/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
var Lookups;
var Menus;
var CloseTabFirst = false;
var DisableFocus = false;
var ConfirmBoxFunction = "";
var TargetId = "";
var RandomRefreshObject = "";
$(document).ready(function () {
    GetMenuList();
    GetLookups();
});
function GetLookups() {
    $.ajax({
        url: "./Data/GetLookups",
        type: "POST",
        dataType: "json",
        success: function (data) {
            Lookups = data;
        }
    });
}
function MenuClick(menuId) {
    AppSpinner(true);
    setTimeout(function () {
        var isRefresh = false;
        var tabName = "tab" + menuId.toString();
        // refresh, set focus if tab already exists
        $(".tab-item").each(function () {
            var thisTabName = $(this).attr("id");
            if (tabName == thisTabName) {
                isRefresh = true;
            }
        });
        // get menu object
        var menu = Menus.find(function (w) { return w.MenuId == menuId; });
        // route menu 
        if (menu.TargetId > 0 && menu.TargetType == "grid") {
            var gridId = menu.TargetId;
            SetPageNavigation(menu.MenuId, menu.MenuTitle, gridId);
            GetGrid(menu.MenuId, true);
        }
        else if (menu.TargetId > 0 && menu.TargetType == "form") {
            $.ajax({
                url: "./Home/GetPage",
                data: { id: menu.TargetId, targetType: menu.TargetType, pageFile: "" },
                type: "POST",
                dataType: "text",
                success: function (response) {
                    AddTab(menu.MenuId, menu.MenuTitle, response);
                }
            });
        }
        else if (menu.PageFile.length > 0 && menu.TargetType == "page") {
            $.ajax({
                url: "./Home/GetPage",
                data: { id: 0, targetType: "", pageFile: menu.PageFile },
                type: "POST",
                dataType: "text",
                success: function (response) {
                    AddTab(menu.MenuId, menu.MenuTitle, response);
                },
                complete: function () {
                }
            });
        }
    }, 300);
}
function AddTab(menuId, menuTitle, content) {
    var isRefresh = false;
    $(".tab-item").each(function () {
        var id = $(this).attr("id").replace("tab", "");
        if (id == menuId) {
            isRefresh = true;
        }
    });
    if (!isRefresh) {
        $("#MainTab").append("<li id='tab" + menuId + "' class='tab-item' onclick=\"PageFocus('" + menuId + "')\">" + menuTitle + "<span class='main-tab-close fa fa-times' onclick=\"CloseTab('" + menuId + "')\"> </span> </li>");
        $("#MainPageContent").append("<li id='page" + menuId + "' class='page-content'>" + content + "</li>");
    }
    else {
        $("#page" + menuId).html(content);
    }
    PageFocus(menuId);
    AppSpinner(false);
}
function PageFocus(menuId) {
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
function CloseTab(menuId) {
    // set focus to previous tab
    var previousMenuLevelId = Number($("#tab" + menuId).prev().attr("id").replace("tab", ""));
    CloseTabFirst = true;
    PageFocus(previousMenuLevelId);
    // remove tab
    $("#tab" + menuId).remove();
    $("#page" + menuId).remove();
}
function SortJson(array, key) {
    return array.sort(function (a, b) {
        var x = a[key];
        var y = b[key];
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
function ConfirmBox(title, msg, confirmBoxFunction) {
    $("#confirmBoxTitle").html(title);
    $("#confirmBoxText").html(msg);
    ConfirmBoxFunction = confirmBoxFunction;
    OpenConfirmBox();
}
function ConfirmMessageYes() {
    eval(ConfirmBoxFunction);
    CloseConfirmBox();
}
function ConfirmMessageNo() {
    CloseConfirmBox();
}
function OpenConfirmBox() {
    $("#overlay").css({ "display": "block" });
    $("#ConfirmBox").css({ "display": "block" });
}
function CloseConfirmBox() {
    $("#overlay").css({ "display": "none" });
    $("#ConfirmBox").css({ "display": "none" });
}
function MessageBox(title, msg, autoClose) {
    $("#messageBoxTitle").html(title);
    $("#messageBoxText").html(msg);
    OpenMessageBox();
    if (autoClose) {
        setTimeout(function () { CloseMessageBox(); }, 1500);
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
    }
    else {
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
function UpdateArray(arrayName, keyName, keyValue, targetName, targetValue) {
    var i = 0;
    var array = eval(arrayName);
    eval("i = " + arrayName + ".findIndex(obj => obj." + keyName + " == " + keyValue + ");");
    if (i > -1) {
        array[i][targetName] = targetValue;
    }
}
function TabIt(id, initIndex) {
    $("#" + id + "Content li").hide();
    // initialize 
    $("#" + id + " li").each(function () {
        if ($(this).index() == initIndex) {
            $(this).addClass("custom-tab-active");
        }
        else {
            $(this).removeClass("custom-tab-active");
        }
    });
    $("#" + id + "Content").children().each(function () {
        if ($(this).index() == initIndex) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
    $("#" + id + " li").click(function () {
        var selectedIndex = $(this).index();
        // set active current
        $("#" + id + " li").each(function () {
            if ($(this).index() == selectedIndex) {
                $(this).addClass("custom-tab-active");
            }
            else {
                $(this).removeClass("custom-tab-active");
            }
        });
        // display content
        $("#" + id + "Content").children().each(function () {
            if ($(this).index() == selectedIndex) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });
    });
}
function AddAlive() {
    var r = Math.floor(Math.random() * 1001);
    RandomRefreshObject = "AmAlive" + r;
    console.log("Setting RandomRefreshObject=" + RandomRefreshObject);
    return "<span id='" + RandomRefreshObject + "'></span>";
}
function RefreshDOM(functionToFire) {
    var myVar = setInterval(function () {
        console.log("RefreshDOM() - Looking for RandomRefreshObject=" + RandomRefreshObject);
        if (RandomRefreshObject.length > 0) {
            if ($("#" + RandomRefreshObject).length) {
                console.log("RefreshDOM() - Found RandomRefreshObject=" + RandomRefreshObject);
                try {
                    eval(functionToFire);
                }
                catch (e) {
                }
                clearInterval(myVar);
            }
        }
    }, 400);
}
//# sourceMappingURL=Index.js.map