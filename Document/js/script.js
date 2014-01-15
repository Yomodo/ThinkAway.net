$(document).ready(function(){
	 //选取导航按钮
    var selectButton = function (nav, btn) {
        $("div[class='menu'] a").removeClass("on");
        $(btn).addClass("on");
    };
	var hideLayer = function(){
		$("body").css("overflow-y","auto");
		$("div[class='layer']").hide();
	}
	var showLayer = function(){
		$("body").css("overflow-y","hidden");
		//$("div[class='layer']").live('click',hideLayer);		
		$("div[class='layer']").show();		
	}
	var showDialog =  function(){		
		$("div[class='dialog_header'] b").live('click',dismissDialog);
		var dialog = $("div[class='dialog']");		
        var offset_x,offset_y,move=false;;
		$("div[class='dialog_header']").click(function(){
		}).mousedown(function(e){
			offset_x=e.pageX-parseInt(dialog.css("left"));
			offset_y=e.pageY-parseInt(dialog.css("top"));
			move=true;
		});
		$(document).mousemove(function(e){
			if(move){
				var x= e.pageX-offset_x;
				var y= e.pageY-offset_y;
				dialog.css({"left":x,"top":y});
			}
		}).mouseup(function(){
			move=false;
		});
		dialog.show();
	}
	
	var dismissDialog = function(){
		hideLayer();
		$("div[class='dialog']").hide();
	}	
	
	$("#btn_dialog").click(function(){
		showLayer();
		showDialog();
	});
	
	
	 $('.menu a').live('click', function () {
        //a -> li => ul (nav)
        var nav = $(this).parent().parent().parent();
        selectButton(nav, this);
    });	
	
	$("#floatMenu").live("")
	
});

function showMenu(){
	$("#floatMenu").show();
	
}

function hideMenu(){
	$("#floatMenu").hide();
}