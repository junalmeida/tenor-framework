// SlidingPanel Web Control
// Author: Marcos A. P. de Almeida Jr.
// Este script foi escrito do modo antigo. No futuro, deverá mudar para prototype.

function SlidingPanel_ResetCenterPanelWidth(idobj, sep) {
    var slide = document.getElementById(idobj);
    var obj = document.getElementById(idobj + sep + "container");
    var objL = document.getElementById(idobj + sep + "left");
    var objR = document.getElementById(idobj + sep + "right");
    var objC = document.getElementById(idobj + sep + "centerPanel");
    objC.style.width = (slide.offsetWidth - objL.offsetWidth - objR.offsetWidth).toString() + "px";
    setTimeout("SlidingPanel_ResetCenterPanelWidth('" + idobj + "', '" + sep + "');", 1000);
    
    if (obj.offsetWidth <= objC.offsetWidth) {
        if (objL) objL.style.visibility="hidden";
        if (objR) objR.style.visibility="hidden";
        obj.style.left="";
        obj.style.margin="auto";
    } else {
        if (objL) objL.style.visibility="visible";
        if (objR) objR.style.visibility="visible";
        obj.style.margin="0px";
        if (!obj.style.left || obj.style.left=="") obj.style.left="0px";
    }
}

function SlidingPanel_SetContentWidth(idobj, sep) {
    if (idobj && sep) {
    
        var slide = document.getElementById(idobj);
        
        var obj = document.getElementById(idobj + sep + "container");
        var objL = document.getElementById(idobj + sep + "left");
        var objR = document.getElementById(idobj + sep + "right");
        var objC = document.getElementById(idobj + sep + "centerPanel");
        var objvalue = document.getElementById(idobj + sep + "Value");
        
        if (slide && objC) {
            if (slide.style.width.indexOf("%") > -1) {
                SlidingPanel_ResetCenterPanelWidth(idobj, sep);
            }
        }
        
        
        if (obj && obj.childNodes) {
  
            var fullwidth = 0;
            for(var i =0; i<obj.childNodes.length;i++) {
                var item = obj.childNodes[i];
                if (item.offsetWidth) {
                    fullwidth = fullwidth + item.offsetWidth;
                }
            }
            if (fullwidth == 0) fullwidth=7000;
            obj.style.width = fullwidth.toString() + "px";
            
            if (objC && (fullwidth <= objC.offsetWidth)) {
                if (objL) objL.style.visibility="hidden";
                if (objR) objR.style.visibility="hidden";
                obj.style.left="";
                obj.style.margin="auto";
            }
            obj.style.top="0px";
            
            if (objvalue && objvalue.value) {
                var valor = Number(objvalue.value)
                if (!isNaN(valor)) {
                    obj.style.left = valor.toString() + "px";
                }
            }
            
        }
        
    }
}

SlidingPanel_MovingOn = false;

function SlidingPanel_Move(obj, offset, max, anim) {
    if (obj && offset && offset != 0) {
        obj = document.getElementById(obj);
        
        obj.style.position = "relative";
        var current = obj.style.left;
        if (current) current = current.replace(/px/i, "");
        current = parseInt(current, 0);
        if (isNaN(current)) current = 0;
        
        var newPosition = current + offset;
        
        //if (offset < 0 && newPosition  < -(obj.offsetWidth-max-2)) newPosition = -(obj.offsetWidth-max-2);
        if (offset < 0) {
            if (newPosition + obj.offsetWidth < obj.parentNode.offsetWidth) {
                newPosition = obj.parentNode.offsetWidth - obj.offsetWidth;
            }
        }


        if (offset > 0 && newPosition > 0) newPosition = 0;

        
        if (anim) {
            setTimeout("SlidingPanel_Anim('" + obj.id + "', " + current.toString() + ", " + newPosition.toString() + ", 20)", 20);
        } else {
            obj.style.left = newPosition.toString() + "px";
            if (SlidingPanel_MovingOn) {
                setTimeout("SlidingPanel_Move('" + obj.id + "', " + offset.toString() + ", " + max.toString() + ", false)", 20);
            }
        }

        var objvalue = document.getElementById(obj.parentNode.parentNode.id + "_Value");
        if (objvalue) {
            objvalue.value = newPosition.toString();
        }

    }
}

function SlidingPanel_Anim(objId, fromPx, toPx, time) {

    
    var obj = document.getElementById(objId);
    var current = obj.style.left;
    if (current) current = current.replace(/px/i, "");
    current = parseInt(current, 0);
    if (isNaN(current)) current = 0;
    
    
    var step = 0;
    step = parseInt((toPx-current) / 2, 0);
    current = current + step;
   
    if (toPx > fromPx) {
        if (step < 5) current = current + 5;
        if (current > toPx) {
            current = toPx;
        }
    } else {
        if (step > -5) current = current - 5;
        if (current < toPx) {
            current = toPx;
        }
    }
    
    obj.style.left = current.toString() + "px";
    if (current != toPx) {
        setTimeout("SlidingPanel_Anim('" + obj.id + "', " + fromPx.toString() + ", " + toPx.toString() + ", " + time.toString() + ")", time);
    }
}


