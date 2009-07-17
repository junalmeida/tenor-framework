// ScrollPanel

function ScrollPanel(id) {
    this.id = id;
    this.panel = document.getElementById(id);
    if (!this.panel) throw "Invalid panel id";
    this.data = document.getElementById(id + "_value");
    if (!this.data) throw "Invalid panel hidden field id";
    
    var oThis = this;
    
    this._SetScroll = function(e) {
        oThis.SetScroll(e);
    }
    
    this._AttachScroll = function(e) {
        oThis.ReadScroll(e);
        oThis.AttachEvent(oThis.panel, "scroll", oThis._SetScroll);
    }
    
    this.AttachEvent(document, "load", this._AttachScroll);
    this.AttachEvent(window, "load", this._AttachScroll); 
}


ScrollPanel.prototype.SetScroll = function() {
    this.data.value = this.panel.scrollLeft.toString() + ";" + this.panel.scrollTop.toString() + ";";
}

ScrollPanel.prototype.ReadScroll = function() {
    if (this.data.value) {
        var values = this.data.value.split(";");
        values[0] = parseInt(values[0]);
        values[1] = parseInt(values[1]);
        this.panel.scrollLeft = values[0];
        this.panel.scrollTop = values[1];

        if ((this.panel.scrollLeft==0) && (this.panel.scrollTop==0) && ((values[0] != 0) || (values[1] != 0))) {
            setTimeout("ScrollPanel_" + this.id + ".ReadScroll()", 100);
        }
    }
}


// Helper functions
ScrollPanel.prototype.AttachEvent = function(obj, eventName, pointer) {
    if (obj.addEventListener) {
        obj.addEventListener(eventName, pointer, false);
    }
    if (obj.attachEvent) {
        obj.attachEvent('on' + eventName, pointer);
    }
}

ScrollPanel.prototype.RemoveEvent = function(obj, eventName, pointer) {
    if (obj.removeEventListener) {
        obj.removeEventListener(eventName, pointer, false);
    }
    if (obj.detachEvent) {
        obj.detachEvent('on' + eventName, pointer);
    }
}
