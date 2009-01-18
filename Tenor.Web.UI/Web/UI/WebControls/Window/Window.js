// Window Web Control
// Author: Marcos A. P. de Almeida Jr.

    function WindowControl (id, movable, resizable, bordersize, titlebarsize, closePostback) {
        this.window = document.getElementById(id);

        if (!this.window) throw "Invalid id";
        this.postData = document.getElementById(id + "_value");
        if (!this.postData) throw "Invalid hidden field id";
        this.postScrollData = document.getElementById(id + "_scroll");
        if (!this.postScrollData) throw "Invalid scroll hidden field id";
        
        this.movable = movable;
        this.resizable = resizable;
        this.sizer = null;
        
        this.title = [];
        this.borderSize = bordersize;
        this.titleBarSize = titlebarsize;
        
        try {
            var myDiv = WindowControl.prototype.getElementsByClassName("title", "tr", this.window)[0];
            for (i in myDiv.childNodes) {
                if (myDiv.childNodes[i].tagName && myDiv.childNodes[i].tagName == "TD")  {
                    this.title.push(myDiv.childNodes[i]);
                }
            }
        } catch (e) { }
        if (this.title.length < 3) throw "Invalid window structure: titlebar";

        
        
        this.content = [];
        try {
            myDiv = WindowControl.prototype.getElementsByClassName("content", "tr", this.window)[0];
            for (i in myDiv.childNodes) {
                if (myDiv.childNodes[i].tagName && myDiv.childNodes[i].tagName == "TD")  {
                    this.content.push(myDiv.childNodes[i]);
                }
            }
        } catch (e) { }
        if (this.content.length < 3) throw "Invalid window structure: content";
        this.divContent = this.content[1].childNodes[0];

        this.footer = [];
        try {
            myDiv = WindowControl.prototype.getElementsByClassName("footer", "tr", this.window)[0];
            for (i in myDiv.childNodes) {
                if (myDiv.childNodes[i].tagName && myDiv.childNodes[i].tagName == "TD")  {
                    this.footer.push(myDiv.childNodes[i]);
                }
            }
        } catch (e) { }
        if (this.content.length < 3) throw "Invalid window structure: footer";
 
        this.closeButton = null;
        this.titleText = null;
        for (i in this.title[1].childNodes) {
            var obj = this.title[1].childNodes[i];
            if (obj.tagName && obj.tagName == "DIV")  {
                if (obj.className == "titleText") {
                    this.titleText = obj;
                } else if (obj.className == "close") {
                    this.closeButton = obj;
                }
            }
        }
       
        myDiv = null;
        
        
        var oThis = this;
        
        this._StartSize = function(event) {
            oThis.StartSize(event);
        }
        
        this._StartSizeX = function(event) {
            oThis.StartSize(event, "x");
        }
        
        this._StartSizeY = function(event) {
            oThis.StartSize(event, "y");
        }
        
        this._StartMove = function(event) {
            oThis.StartSize(event, "move");
        }
        
        this._SizeGo = function(event) {
            oThis.SizeGo(event, null);
        }
        
        this._SizeGoX = function(event) {
            oThis.SizeGo(event, "x");
        }
        
        this._SizeGoY = function(event) {
            oThis.SizeGo(event, "y");
        }
        
        this._MoveGo = function(event) {
            oThis.SizeGo(event, "move");
        }
        
        this._SizeStop = function(event) {
            oThis.SizeStop(event);
        }
        
        this._DoResize = function(event) {
            oThis.DoResize(event);
        }
        
        this._DoClose = function(event) {
            oThis.DoClose(event);
        }
        
        this._DoScroll = function(event) {
            oThis.DoScroll(event);
        }
        
        this._DoRedraw = function(event) {
            oThis.DoRedraw(event);
        }

        if (this.resizable) {
            this.content[2].style.cursor = "w-resize";
            
            this.footer[1].style.cursor = "n-resize";
            
            this.footer[2].style.cursor = "nw-resize";
            
            this.AttachEvent(this.footer[2], "mousedown", this._StartSize);
            this.AttachEvent(this.content[2], "mousedown", this._StartSizeX);
            this.AttachEvent(this.footer[1], "mousedown", this._StartSizeY);
        }
        
        if (this.movable) {
            this.title[1].style.cursor="move";
            this.AttachEvent(this.title[1], "mousedown", this._StartMove);
        }
        
        if (this.closeButton) {
            this.closePostback = closePostback;
            this.AttachEvent(this.closeButton, "mousedown", this._DoClose);
        }

        this.CreateSizer();
        this.DoResize('startup');
        this.ResizeTimeout();
        this.RestoreScroll();
        this.AttachEvent(this.divContent, "scroll", this._DoScroll);
    }

    
    WindowControl.prototype.RestoreScroll = function() {
        var valores = this.postScrollData.value;
        if (valores && valores.length > 0) {
            try {
                var pos = valores.split(";");
                
                this.divContent.scrollLeft = pos[0];
                this.divContent.scrollTop = pos[1];
            } catch (e) { } 
        }
    }
    
    WindowControl.prototype.DoScroll = function() {
        this.postScrollData.value = this.divContent.scrollLeft.toString() + ";" + this.divContent.scrollTop.toString() + ";";
    }
    
    WindowControl.prototype.DoClose = function() {
        this.window.style.display = "none";
        this.DoResize('window');
        if (this.closePostback) {
            eval(this.closePostback);
        }
    }
    
    WindowControl.prototype.CreateSizer = function() {
        if (!this.sizer) {
            var obj = document.createElement("div");
            obj.style.display = "none";
            obj.style.border = "dotted 1px black";
            obj.style.zIndex = "1000000";
            
            document.body.insertBefore(obj, document.body.lastChild);            
            
            this.sizer=obj;
            
            this.SizeSizer();
        }
    }
    
    WindowControl.prototype.ShowSizer = function(show) {
        if (this.sizer) {
            if (show) {
                this.sizer.style.display = "";
                this.DisableSelect();
            } else {
                this.sizer.style.display = "none";
                this.EnableSelect();
            }
        }
    }    

   WindowControl.prototype.CannotSelect = function(e){
        return false;
   } 

   WindowControl.prototype.CanSelect = function(e){
        return false;
   } 

   WindowControl.prototype.DisableSelect = function(e){
        //if IE4+
        if (document.all) {
            document.onselectstart=new Function ("return WindowControl.prototype.CannotSelect(event);");
        }
        //if NS6
        if (window.sidebar){
            document.onmousedown=WindowControl.prototype.CannotSelect;
            document.onclick=WindowControl.prototype.CanSelect;
        }
   } 

   WindowControl.prototype.EnableSelect = function(e){
        //if IE4+
        if (document.all) {
            document.onselectstart=null;
        }
        //if NS6
        if (window.sidebar){
            document.onmousedown=null;
            document.onclick=null;
        }
   } 
    
      
    WindowControl.prototype.SizeSizer = function() {
        if (this.sizer) {
            var posObj = WebForm_GetElementPosition(this.window);
            this.sizer.style.position = "absolute";
            this.sizer.style.left = posObj.x.toString() + "px";
            this.sizer.style.top = posObj.y.toString() + "px";
            this.sizer.style.width = posObj.width.toString() + "px";
            this.sizer.style.height = (posObj.height).toString() + "px";
        }
    }    
    
    WindowControl.prototype.StartSize = function(e, mode) {
        if (this.window.style.display == "none") return;


        this.ShowSizer(true);
        if (!mode) {
            this.AttachEvent(document, "mousemove", this._SizeGo);
        } else if (mode == "x") {
            this.AttachEvent(document, "mousemove", this._SizeGoX);
        } else if (mode == "y") {
            this.AttachEvent(document, "mousemove", this._SizeGoY);
        } else if (mode == "move") {
            var posObj = WebForm_GetElementPosition(this.window);
            if (!e) e = window.event;
            var x, y;
            x = e.clientX + document.documentElement.scrollLeft + document.body.scrollLeft;
            y = e.clientY + document.documentElement.scrollTop + document.body.scrollTop;
            
            this.offsetX = x - posObj.x           
            this.offsetY = y - posObj.y           
            this.AttachEvent(document, "mousemove", this._MoveGo);
        }
        this.AttachEvent(document, "mouseup",  this._SizeStop);
    }

   
    WindowControl.prototype.SizeGo = function(e, mode) {
        var x, y;
        if (!e) e = window.event;
        x = e.clientX + document.documentElement.scrollLeft + document.body.scrollLeft;
        y = e.clientY + document.documentElement.scrollTop + document.body.scrollTop;
        
        var posObj = WebForm_GetElementPosition(this.sizer);
        
        if (mode && mode == "move") {
            var left = x - this.offsetX;
            var top = y - this.offsetY;
            
            
            this.sizer.style.left = left.toString() + "px";    
            this.sizer.style.top = top.toString() + "px";    
            
        } else {


            var width = x - posObj.x;
            var height = y - posObj.y;
            
            if (width < 100) width = 100;
            if (height < 100) height = 100;
           
            if (!mode || mode == "x") {
                this.sizer.style.width = width.toString() + "px";    
            }
            if (!mode || mode == "y") {
                this.sizer.style.height = height.toString() + "px";    
            }
            
            e.cancelBubble = true;
            e.returnValue = false;
        }
        
    }

    WindowControl.prototype.SizeStop = function(e) {
        this.RemoveEvent(document, "mousemove", this._SizeGo);
        this.RemoveEvent(document, "mousemove", this._SizeGoX);
        this.RemoveEvent(document, "mousemove", this._SizeGoY);
        this.RemoveEvent(document, "mousemove", this._MoveGo);
        this.RemoveEvent(document, "mouseup", this._SizeStop);
        
        this.DoResize(e);
        this.ShowSizer(false);
        this.ResizeTimeout(3);
    }
    
    WindowControl.prototype.ResizeTimeout = function(repeat) {
        try {
            for (var i = 1; i <= repeat; i++) {
                setTimeout("WindowControl.prototype.ResizeTimer('" + this.window.id + "')", 50 * i);
            }
        } catch (e) {
        }
    }
    
    WindowControl.prototype.ResizeTimer = function(id) {
        var oThis = null;
        eval("oThis = Window_" + id);
        if (oThis) oThis.DoResize('window');
    }
    
    WindowControl.prototype.DoResize = function(e) {
        if (this.window.style.display == "none") {
            this.postData.value = this.postData.value.replace(/;false;/ig, ";true;");
            return;
        }
        
        var objPos 
        if (e && (e=="window" || e =="startup")) {
            objPos = WebForm_GetElementPosition(this.window);
        } else {
            this.window.style.width = this.sizer.style.width;
            this.window.style.height = this.sizer.style.height;
            this.window.style.left = this.sizer.style.left;
            this.window.style.top = this.sizer.style.top;
            objPos = WebForm_GetElementPosition(this.sizer);
        }
        
        if (e && e=="startup") {
            if (this.postData.value == "center;") {
                var obj = this.getViewPort();
                this.window.style.left = ((obj.width / 2) - (objPos.width / 2)).toString() + "px";
                this.window.style.top = ((obj.height / 2) - (objPos.height / 2)).toString() + "px";
                objPos = WebForm_GetElementPosition(this.window);
            }
        }
    
    
    

        var contentPos = parseInt(this.window.style.height.replace(/px/ig, "")) - this.titleBarSize - this.borderSize;
		this.content[1].childNodes[0].style.height = contentPos.toString() + "px";
		this.content[1].childNodes[0].style.maxHeight = contentPos.toString() + "px";
		this.content[1].style.height = contentPos.toString() + "px";
		this.content[1].style.maxHeight = contentPos.toString() + "px";
		
		       
        this.SizeSizer();   
        
        var postData = objPos.x.toString() + ";" + objPos.y.toString() + ";" + objPos.width.toString() + ";" + objPos.height.toString() + ";";
        if (this.window.style.display.toString().toLowerCase() == "none") {
            postData += "true;";
        } else { 
            postData += "false;";
        }
        this.postData.value = postData;
    }

    
    
    // Helper functions
    WindowControl.prototype.AttachEvent = function(obj, eventName, pointer) {
		if (obj) {
			if (obj.addEventListener) {
				obj.addEventListener(eventName, pointer, false);
			}
			if (obj.attachEvent) {
				obj.attachEvent('on' + eventName, pointer);
			}
		}
    }

    WindowControl.prototype.RemoveEvent = function(obj, eventName, pointer) {
		if (obj) {
			if (obj.removeEventListener) {
				obj.removeEventListener(eventName, pointer, false);
			}
			if (obj.detachEvent) {
				obj.detachEvent('on' + eventName, pointer);
			}
		}
    }
    
    
    
    WindowControl.prototype.getViewPort = function() {
         var result = new Object();
     
         // the more standards compliant browsers (mozilla/netscape/opera/IE7) use window.innerWidth and window.innerHeight
         
         if (typeof window.innerWidth != 'undefined')
         {
              result.width = (window.innerWidth);
              result.height = (window.innerHeight);
         }
         
        // IE6 in standards compliant mode (i.e. with a valid doctype as the first line in the document)

         else if (typeof document.documentElement != 'undefined'
             && typeof document.documentElement.clientWidth !=
             'undefined' && document.documentElement.clientWidth != 0)
         {
               result.width = (document.documentElement.clientWidth);
               result.height = (document.documentElement.clientHeight);
         }
         
         // older versions of IE
         
         else
         {
               result.width = (document.getElementsByTagName('body')[0].clientWidth);
               result.height = (document.getElementsByTagName('body')[0].clientHeight);
         }
         return result;
    }



    WindowControl.prototype.getElementsByClassName = function(className, tag, elm) {
	    var testClass = new RegExp("(^|\\s)" + className + "(\\s|$)");
	    var tag = tag || "*";
	    var elm = elm || document;
	    var elements = (tag == "*" && elm.all)? elm.all : elm.getElementsByTagName(tag);
	    var returnElements = [];
	    var current;
	    var length = elements.length;
	    for(var i=0; i<length; i++){
		    current = elements[i];
		    if(testClass.test(current.className)){
			    returnElements.push(current);
		    }
	    }
	    return returnElements;
    }
