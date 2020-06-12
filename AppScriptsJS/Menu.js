function GetMenuList() {
    $.ajax({
        url: "./Menu/GetMenuList",
        dataType: "json",
        success: function (data) {
            var records = data;
            var obj = [];
            obj.push("<ul>");
            // record rows
            for (var i = 0; i < records.length; i++) {
                var row = records[i];
                Menu1Items.push(row);
                obj.push("<li onclick='MenuClick(1, " + row.Menu1ItemId + ", false)'>"); // level one  MenuClick(level: number, id: number, isRefresh: boolean)
                obj.push(row.MenuTitle);
                if (row.Menu2Item.length > 0) {
                    obj.push("<ul>");
                    for (var x = 0; x < row.Menu2Item.length; x++) {
                        var row2 = row.Menu2Item[x];
                        Menu2Items.push(row2);
                        obj.push("<li onclick='MenuClick(2, " + row2.Menu2ItemId + ", false)'>"); // level two
                        obj.push(row2.MenuTitle);
                        if (row2.Menu3Item.length > 0) {
                            obj.push("<ul>");
                            for (var y = 0; y < row2.Menu3Item.length; y++) {
                                var row3 = row2.Menu3Item[y];
                                Menu3Items.push(row3);
                                obj.push("<li onclick='MenuClick(3, " + row3.Menu3ItemId + ", false)'>"); // level three
                                obj.push(row3.MenuTitle);
                                obj.push("</li>");
                            }
                            obj.push("</ul>");
                        }
                        obj.push("</li>");
                    }
                    obj.push("</ul>");
                }
                obj.push("</li>");
            }
            obj.push("</ul>");
            var str = obj.join("");
            $("#main_nav").html(str);
        }
    });
}
//# sourceMappingURL=Menu.js.map