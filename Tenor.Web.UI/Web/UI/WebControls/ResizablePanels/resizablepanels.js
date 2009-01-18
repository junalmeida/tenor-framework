// JScript File

function ResizablePanels(id, orientation) {
    //usar o metodo OnResize para pegar o evento resize
    //function(value)

    this.OnResize = function() {
    };

    this.orientation = orientation;
    this.id = id;
    this.panel = document.getElementById(id);
    if (!this.panel) throw "ResizablePanels: Invalid id.";
    this.data = document.getElementById(id + "_value");
    if (!this.data) throw "ResizablePanels - '" + id + "': Cannot find hidden field.";
    this.first = document.getElementById(id + "_first");
    if (!this.first) throw "ResizablePanels - '" + id + "': Invalid structure.";
    this.second = document.getElementById(id + "_second");
    if (!this.second) throw "ResizablePanels - '" + id + "': Invalid structure.";
    this.divider = document.getElementById(id + "_div");
    if (!this.divider) throw "ResizablePanels - '" + id + "': Invalid structure.";
    
    var oThis = this;
    this._DoResize = function(event) {
        oThis.DoResize(event);
    };
    
    this._StartSize = function(event) {
        oThis.StartSize(event);
    };
    
    this._SizeGo = function(event) {
        oThis.SizeGo(event);
    };
    this._SizeStop = function(event) {
        oThis.SizeStop(event);
    };
    
    this._InitialResize = function(event) {
        oThis.DoResize(null);
        oThis.AttachEvent(oThis.panel, "resize", oThis._DoResize);
        oThis.AttachEvent(oThis.panel, "scroll", oThis._DoResize);
        oThis.AttachEvent(oThis.divider, "mousedown", oThis._StartSize);
    };

    
    this.AttachEvent(document, "load", this._InitialResize);
    this.AttachEvent(window, "load", this._InitialResize);
}


ResizablePanels.prototype.DoResize = function() {
    var posDiv = WebForm_GetElementPosition(this.divider);
    var posObj = WebForm_GetElementPosition(this.panel);
    if (this.orientation == "h") {
        var value = (posObj.width - (posDiv.x - posObj.x) - 7);
        if (value < 1) {
            value = 1;
        }
        this.second.style.width = value.toString() + "px";
        this.data.value =  (posDiv.x - posObj.x).toString();
    } else {
        var value = (posObj.height - (posDiv.y - posObj.y) - 7);
        if (value < 1) {
            value = 1;
        }
        this.second.style.height = value.toString() + "px";
        this.data.value =  (posDiv.y - posObj.y).toString();
    }
    if (this.OnResize) {
        this.OnResize(parseInt(this.data.value));
    }
}


ResizablePanels.prototype.StartSize = function(e){
    if (document.body) {
        if (this.orientation=="v") {
            document.body.style.cursor="n-resize";
        } else {
            document.body.style.cursor="w-resize";
        }
    }
            
    this.AttachEvent(document, "mousemove", this._SizeGo);
    this.AttachEvent(document, "mouseup",  this._SizeStop);
//    window.event.cancelBubble = true;
//    window.event.returnValue = false;

}

ResizablePanels.prototype.SizeGo = function(e) {
    var x, y;
    if (!e) e = window.event;
    x = e.clientX + document.documentElement.scrollLeft + document.body.scrollLeft;
    y = e.clientY + document.documentElement.scrollTop + document.body.scrollTop;


    var posObj = WebForm_GetElementPosition(this.panel);
    
    var size = 200;
    if (this.orientation == "h") {
        size = (x - posObj.x);
        
        if (size < 100) size = 100;
        if (size > (posObj.width-100)) size = posObj.width-100;
        this.first.style.width =  size.toString() + "px";
    } else { 
        size = (y - posObj.y);

        if (size < 100) size = 100;
        if (size > (posObj.height-100)) size = posObj.height-100;
        this.first.style.height =  size.toString() + "px";
    }
    
    
    e.cancelBubble = true;
    e.returnValue = false;
    
    this.DoResize(e);
    
}

ResizablePanels.prototype.SizeStop = function(e) {
    this.RemoveEvent(document, "mousemove", this._SizeGo);
    this.RemoveEvent(document, "mouseup", this._SizeStop);
    document.body.style.cursor="";
}







// Helper functions
ResizablePanels.prototype.AttachEvent = function(obj, eventName, pointer) {
    if (obj.addEventListener) {
        obj.addEventListener(eventName, pointer, false);
    }
    if (obj.attachEvent) {
        obj.attachEvent('on' + eventName, pointer);
    }
}

ResizablePanels.prototype.RemoveEvent = function(obj, eventName, pointer) {
    if (obj.removeEventListener) {
        obj.removeEventListener(eventName, pointer, false);
    }
    if (obj.detachEvent) {
        obj.detachEvent('on' + eventName, pointer);
    }
}


ResizablePanels.prototype.getViewPort = function() {
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
