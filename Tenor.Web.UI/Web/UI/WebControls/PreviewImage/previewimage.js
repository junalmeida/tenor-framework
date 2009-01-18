//Marcos A. P. de Almeida Jr.


function getPageScroll() {
 var yScroll;
 if (self.pageYOffset) {
     yScroll = self.pageYOffset;
 } else if (document.documentElement && document.documentElement.scrollTop){
     yScroll = document.documentElement.scrollTop;
 } else if (document.body) {
     yScroll = document.body.scrollTop;
 }
 arrayPageScroll = new Array('',yScroll)
 return arrayPageScroll;
}


//
// getPageSize()
// Returns array with page width, height and window width, height
// Core code from - quirksmode.org
// Edit for Firefox by pHaez
//
function getPageSize(){
    var arrayPageSize;
	var xScroll, yScroll;
	if (window.innerHeight && window.scrollMaxY) {	
		xScroll = document.body.scrollWidth;
		yScroll = window.innerHeight + window.scrollMaxY;
	} else if (document.body.scrollHeight > document.body.offsetHeight){ // all but Explorer Mac
		xScroll = document.body.scrollWidth;
		yScroll = document.body.scrollHeight;
	} else { // Explorer Mac...would also work in Explorer 6 Strict, Mozilla and Safari
		xScroll = document.body.offsetWidth;
		yScroll = document.body.offsetHeight;
	}
	var windowWidth, windowHeight;
	if (self.innerHeight) {	// all except Explorer
		windowWidth = self.innerWidth;
		windowHeight = self.innerHeight;
	} else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
		windowWidth = document.documentElement.clientWidth;
		windowHeight = document.documentElement.clientHeight;
	} else if (document.body) { // other Explorers
		windowWidth = document.body.clientWidth;
		windowHeight = document.body.clientHeight;
	}	
	// for small pages with total height less then height of the viewport
	if(yScroll < windowHeight){
		pageHeight = windowHeight;
	} else { 
		pageHeight = yScroll;
	}
	
	try {
	    if (forcePageH) {
	        if (forcePageH > pageHeight) {
	            pageHeight = forcePageH;
	        }
	    }
	} catch(e) { }

	// for small pages with total width less then width of the viewport
	if(xScroll < windowWidth){	
		pageWidth = windowWidth;
	} else {
		pageWidth = xScroll;
	}
	arrayPageSize = new Array(pageWidth,pageHeight,windowWidth,windowHeight) 
	return arrayPageSize;
}

//Original: Macromedia
function PreviewImage_Preload() {

  if(document.images) {

    if(!document.imageArray) document.imageArray = new Array();
    var i,j = document.imageArray.length, args = PreviewImage_Preload.arguments;

    for(i=0; i<args.length; i++) {
      if (args[i].indexOf("#")!=0) {
        document.imageArray[j] = new Image;
        document.imageArray[j++].src = args[i];
      }
    }

  }
}

function PreviewImage_BackObj(html, body, pageinfo, TValue, TColor) {
    
    var backobj = document.createElement("DIV");
    backobj.style.position="absolute";
    backobj.style.top="0px";
    backobj.style.left="0px";
    backobj.style.width="100%";
    backobj.style.height=pageinfo[1].toString() + "px";
   
    if (!TColor) TColor = "black";
    
    
    if (!TValue) TValue = 30;
    
    try {
        backobj.style.filter = "Alpha(Opacity=" + TValue.toString() + ")";  
    } catch (e) {} 
    try {
        backobj.style.mozOpacity = (TValue / 100).toString();
    } catch (e) {} 
    try {
        backobj.style.opacity = (TValue / 100).toString();
    } catch (e) {} 
    
    backobj.style.backgroundColor=TColor;
    backobj.style.textAlign = "center";
    
    body.appendChild(backobj);
}

function PreviewImage_Open(Obj, Url, Desc, Loading, CloseImg, TValue, TColor, HideObjects) {
    var pageinfo = getPageSize();
    var pagescroll = getPageScroll();
    
    var html = document.getElementsByTagName("html")[0];
    var body = document.getElementsByTagName("body")[0];
    
    PreviewImage_BackObj(html, body, pageinfo, TValue, TColor);
    

    if (HideObjects) PreviewImage_Flash(false);
       
    //var pos = WebForm_GetElementPosition(Obj);
    
    var centerdiv = document.createElement("div")
    centerdiv.style.position="absolute";
    centerdiv.style.left="0px"
    
    centerdiv.style.width="100%";
    centerdiv.style.textAlign="center";
    //centerdiv.style.zIndex="100000";
    
    
    if (Loading) {
        var imgl = new Image();
        imgl.onload=function(){
            var imgTop=0;
            imgTop = pagescroll[1] + ((pageinfo[3] - 35 - imgl.height) / 2);
            centerdiv.style.top=imgTop.toString() + "px";
            centerdiv.style.height=imgl.height.toString() + "px";
            centerdiv.style.backgroundImage="url(" + Loading + ")";
            centerdiv.style.backgroundRepeat="no-repeat";
            centerdiv.style.backgroundPosition="center";
            imgl.onload=function(){};
            body.appendChild(centerdiv);
            CreateImage(pagescroll, pageinfo, html, body, centerdiv, Url, Loading, CloseImg, Desc)
            return false;
        };
        imgl.src=Loading;
        
    } else {
        centerdiv.style.top = "0px";
        CreateImage(pagescroll, pageinfo, html, body, centerdiv, Url, Loading, CloseImg, Desc)
    }
    
}

function CreateImage(pagescroll, pageinfo, html, body, centerdiv, Url, Loading, CloseImg, Desc) {
    var img = document.createElement("IMG");
    img.id="img_PreviewImage_Open_centerdiv";
    img.style.visibility="hidden";
    img.style.padding="10px 10px 30px 10px";
    img.style.backgroundColor="white";
    img.style.border="solid 1px black";
    
    img.style.cursor="pointer";
    img.onclick = PreviewImage_Close;
    centerdiv.onclick = PreviewImage_Close;
    imgPreloader = new Image();
    imgPreloader.onload=function(){
        var imgTop=0;
        imgTop = pagescroll[1] + ((pageinfo[3] - 35 - imgPreloader.height) / 2);
        centerdiv.style.height=imgPreloader.height.toString() + "px";
        centerdiv.style.top=imgTop.toString() + "px";
        
        
        img.src=Url;
        if (!Loading) {
            body.appendChild(centerdiv);
        } 
        centerdiv.appendChild(img);
        imgPreloader.onload=function(){};
        
        if (Desc) {
            Desc = Desc.replace(/\r/ig, "\\r");
            Desc = Desc.replace(/\n/ig, "\\n");
            Desc = Desc.replace(/\'/ig, "\\'");
            Desc = "'" + Desc + "'";
        } else {
            Desc = "null";
        }
        if (CloseImg) {
            CloseImg = "'" + CloseImg + "'";
        } else {
            CloseImg = "null";
        }
        setTimeout("PreviewImage_Show(" + CloseImg + ", " + Desc + ");", 500);
        
        return false;
    };
    imgPreloader.src = Url;
}

function PreviewImage_Show(CloseImg, Desc) {
var img = document.getElementById('img_PreviewImage_Open_centerdiv');
if (img)
    img.style.visibility="visible";

    if (CloseImg) {
        var imgclose = document.createElement("IMG");
        imgclose.src = CloseImg;
        imgclose.style.position="absolute";
        var pos = WebForm_GetElementPosition(img);
        imgclose.style.top="0px";
        imgclose.style.left=(pos.x + pos.width - 26).toString() + "px";
        imgclose.onclick=PreviewImage_Close;
        imgclose.title="Clique para fechar";
        imgclose.style.cursor="pointer";
        
        if (img)
            img.parentNode.appendChild(imgclose);
        
    }
    if (Desc) {
        var lbl = document.createElement("SPAN");
        lbl.className = "PreviewImage_Desc";
        lbl.style.position="relative";
        lbl.style.top="-20px";
        lbl.style.cursor="arrow";
        lbl.innerHTML="<br />" + Desc;
        
        img.parentNode.appendChild(lbl);
    }

}

function PreviewImage_Close() {
    var html = document.getElementsByTagName("html")[0];
    var body = document.getElementsByTagName("body")[0];

    body.removeChild(body.lastChild);
    body.removeChild(body.lastChild);
    
    PreviewImage_Flash(true);
}


//var PreviewImage_Objs = new Array();
//var PreviewImage_Embeds = new Array();
//var PreviewImage_ObjsOld = new Array();
//var PreviewImage_EmbedsOld = new Array();


var PreviewImage_Selects = new Array();

function PreviewImage_Flash(show){
    var userAgent = navigator.userAgent;
    var versionOffset = userAgent.indexOf("MSIE");
    var isIE = (versionOffset >= 0);
    var isPreIE7 = false;
    var fullVersionIE = "";
    var majorVersionIE = "";
    if (isIE)
    {
        fullVersionIE = parseFloat(userAgent.substring(versionOffset+5, userAgent.length));
        majorVersionIE = parseInt('' + fullVersionIE);
        isPreIE7 = majorVersionIE < 7;
    }



    var flashs = document.getElementsByTagName("object");
    for (i = 0; i < flashs.length; i++) {
        flashs[i].style.visibility = (show?"visible":"hidden");
    }
    if (isPreIE7) {
        if (!show) {
            PreviewImage_Selects = new Array();
            var selects = document.getElementsByTagName("select");
            for (i = 0; i < selects.length; i++) {
                if (selects[i].style.visibility.toLowerCase() != "hidden") {
                    selects[i].style.visibility = "hidden";
                    PreviewImage_Selects.push(selects[i]);
                }
            }
        } else {
            do {
                var obj = PreviewImage_Selects.pop();
                if (!obj) break;
                obj.style.visibility="visible";
            } while(true);
        }
    }
}