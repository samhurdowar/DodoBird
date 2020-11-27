var SortableMenuItems = "";
var ObjMenu = [];
var HighlightedItem = "";
function GetAdminMenuList(refreshId) {
    $.ajax({
        url: "./Menu/GetAdminMenuList",
        dataType: "json",
        success: function (data) {
            SortableMenuItems = "RootMenus0";
            ObjMenu.length = 0;
            // get main menus
            var refreshMenuId = refreshId.replace("RootMenus", "").replace("MenuId", "");
            var mainMenus;
            if (refreshMenuId == 0) {
                mainMenus = data.filter(function (it) { return it.ParentId == 0; });
            }
            else {
                mainMenus = data.filter(function (it) { return it.MenuId == refreshMenuId; });
            }
            mainMenus = SortJson(mainMenus, "SortOrder");
            for (var i in mainMenus) {
                var row = mainMenus[i];
                var subMenus = data.filter(function (w) { return w.ParentId == row.MenuId; });
                subMenus = SortJson(subMenus, "SortOrder");
                if (subMenus.length > 0) {
                    ObjMenu.push("<li id='MenuId" + row.MenuId + "' class='refresh-item'>");
                    ObjMenu.push("<div id='Highlight" + row.MenuId + "' class='parent-menu-item'><span highlight='Highlight" + row.MenuId + "' menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuTitle + "</span> <span class='menu-add-delete'> <span class='menu-item-command fas fa-plus'>&nbsp;</span> <span class='menu-item-command fas fa-trash'>&nbsp;</span></span></div>");
                    GetAdminSubMenuList(row.MenuId, subMenus, data);
                    ObjMenu.push("</li>");
                }
                else {
                    ObjMenu.push("<li id='MenuId" + row.MenuId + "' class='refresh-item menu-item'>");
                    ObjMenu.push("<span highlight='MenuId" + row.MenuId + "' menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuTitle + "</span>  <span class='menu-add-delete'> <span class='menu-item-command fas fa-plus'>&nbsp;</span> <span class='menu-item-command fas fa-trash'>&nbsp;</span></span>");
                    ObjMenu.push("</li>");
                }
            }
            var str = ObjMenu.join("");
            $("#" + refreshId).html(str);
            RefreshMenuDOM();
            HighlightMenuItem();
        }
    });
}
function GetAdminSubMenuList(menuId, subMenus, data) {
    SortableMenuItems += ",Children" + menuId;
    ObjMenu.push("<ul id='Children" + menuId + "'>");
    // get sub menus
    for (var x in subMenus) {
        var row = subMenus[x];
        var subMenus_ = data.filter(function (w) { return w.ParentId == row.MenuId; });
        subMenus_ = SortJson(subMenus_, "SortOrder");
        if (subMenus_.length > 0) {
            ObjMenu.push("<li id='MenuId" + row.MenuId + "'>");
            ObjMenu.push("<div id='Highlight" + row.MenuId + "' class='parent-menu-item'>");
            ObjMenu.push("<span highlight='Highlight" + row.MenuId + "' menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuTitle + "</span> <span class='menu-add-delete'> <span class='menu-item-command fas fa-plus'>&nbsp;</span> <span class='menu-item-command fas fa-trash'>&nbsp;</span></span>");
            ObjMenu.push("</div>");
            GetAdminSubMenuList(row.MenuId, subMenus_, data);
            ObjMenu.push("</li>");
        }
        else {
            ObjMenu.push("<li id='MenuId" + row.MenuId + "' class='menu-item'><span highlight='MenuId" + row.MenuId + "' menuId='" + row.MenuId + "' class='menu-item-edit'>" + row.MenuTitle + "</span>  <span class='menu-add-delete'> <span class='menu-item-command fas fa-plus'>&nbsp;</span> <span class='menu-item-command fas fa-trash'>&nbsp;</span></span></li>");
        }
    }
    ObjMenu.push("</ul>");
}
function RefreshMenuDOM() {
    var words = SortableMenuItems.split(",");
    for (var i = 0; i < words.length; i++) {
        $("#" + words[i]).sortable({
            stop: function (event, ui) {
                var menuId = $(ui.item).attr("id").replace("MenuId", "");
                var newOrder = ui.item.index() + 1;
                // get parent item to refresh
                var obj = $(this).closest("li.refresh-item");
                var refreshId = (obj.attr("id") == undefined) ? "RootMenus0" : obj.attr("id");
                console.log("SortMenu    menuId=" + menuId + "    newOrder=" + newOrder + "    refreshId=" + refreshId);
                SortMenu(menuId, newOrder, refreshId);
            }
        });
    }
    // show add and delete
    $(".parent-menu-item").mouseover(function () {
        $(this).closest('div').find(".menu-add-delete:first").show();
    });
    $(".parent-menu-item").mouseout(function () {
        $(this).closest('div').find(".menu-add-delete:first").hide();
    });
    $(".menu-item").mouseover(function () {
        $(this).closest('li').find(".menu-add-delete:first").show();
    });
    $(".menu-item").mouseout(function () {
        $(this).closest('li').find(".menu-add-delete:first").hide();
    });
    // edit menu item
    $(".menu-item-edit").click(function () {
        var menuId = $(this).attr("menuId");
        // get parent item to refresh
        var obj = $(this).closest("li.refresh-item");
        var refreshId = obj.attr("id");
        console.log("SelectMenu menuId=" + menuId + "    refreshId=" + refreshId);
        // set highlight item
        var highlight = $(this).attr("highlight");
        HighlightedItem = highlight;
        HighlightMenuItem();
        SelectMenu(menuId, refreshId);
    });
}
function HighlightMenuItem() {
    $(".menu-item").removeClass("menu-item-active");
    $(".parent-menu-item").removeClass("menu-item-active");
    $("#" + HighlightedItem).addClass("menu-item-active");
}
function SortMenu(menuId, newOrder, refreshId) {
    $.ajax({
        url: "./Menu/SortMenu",
        data: { menuId: menuId, newOrder: newOrder },
        dataType: "text",
        success: function (data) {
            GetAdminMenuList(refreshId);
        }
    });
}
function SelectMenu(menuId, refreshId) {
    $.ajax({
        url: "./Menu/GetMenuItem",
        data: { menuId: menuId },
        type: "POST",
        dataType: "json",
        success: function (data) {
            BindForm("EditMenu", data[0]);
            // set display of TargetType options
            $(".targetType-class").hide();
            $("#span_" + data[0].TargetType).show();
        }
    });
}
function GetMenuList() {
    $.ajax({
        url: "./Menu/GetMenuList",
        dataType: "json",
        success: function (data) {
            Menus = data;
            ObjMenu.length = 0;
            ObjMenu.push("<ul>");
            // get main menus
            var mainMenus = data.filter(function (it) { return it.ParentId == 0; });
            mainMenus = SortJson(mainMenus, "SortOrder");
            for (var i in mainMenus) {
                var row = mainMenus[i];
                var subMenus = data.filter(function (w) { return w.ParentId == row.MenuId; });
                if (subMenus.length > 0) {
                    ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
                    ObjMenu.push(row.MenuTitle);
                    GetSubMenuList(subMenus, data);
                    ObjMenu.push("</li>");
                }
                else {
                    ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
                    ObjMenu.push(row.MenuTitle);
                    ObjMenu.push("</li>");
                }
            }
            ObjMenu.push("</ul>");
            var str = ObjMenu.join("");
            $("#main_nav").html(str);
        }
    });
}
function GetSubMenuList(subMenus, data) {
    ObjMenu.push("<ul>");
    // get sub menus
    for (var x in subMenus) {
        var row = subMenus[x];
        var subMenus_ = data.filter(function (w) { return w.ParentId == row.MenuId; });
        subMenus_ = SortJson(subMenus_, "SortOrder");
        if (subMenus_.length > 0) {
            ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
            ObjMenu.push(row.MenuTitle + "&nbsp;&nbsp;<span class='menu-item-command fas fa-caret-right'>&nbsp;</span>");
            GetSubMenuList(subMenus_, data);
            ObjMenu.push("</li>");
        }
        else {
            ObjMenu.push("<li onclick='MenuClick(" + row.MenuId + ")'>");
            ObjMenu.push(row.MenuTitle);
            ObjMenu.push("</li>");
        }
    }
    ObjMenu.push("</ul>");
}
//# sourceMappingURL=Menu.js.map