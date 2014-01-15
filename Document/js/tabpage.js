
var ready = $(document).ready(function () {

    
    //select tab
    var selectTab = function (tabName) {
        if ($("#" + tabName).length > 0) {
            // hide other tabs
            $("#tabs li").removeClass("current");

            $("#" + tabName).addClass("current");
            //
            $("#tabPage div[tabpagename]").hide();

            $("#tabPage div[tabpagename='" + tabName + "']").show();
        }
    };
    var addTab = function (tabName, tabText) {
        //if not find
        if ($("#" + tabName).length == 0) {
            $("#tabs").append("<li id='" + tabName + "'>" + tabText + "&nbsp;<a class='remove'>x</a></li>");
        }
        selectTab(tabName);
    };
    //remove tabs
    var removeTab = function (tabName) {
        $("#" + tabName).remove();
        //
        if ($("#tabs li.current").length == 0) {
            selectFirstTab();
        }
    };
    //select tabs
    var selectFirstTab = function () {
        var firstTab = $("#tabs li:first-child");
        selectTab($(firstTab).attr("id"));
    };

    $('#tabs li').live('click', function () {
        selectTab($(this).attr("id"));
    });

    $('#tabs a.remove').live('click', function () {
        removeTab($(this).parent().attr("id"));
    });

   
    //set rel to tab
    $('.nav li a[rel]').live('click', function () {
        addTab($(this).attr("rel"), $(this).html());
    });

});
