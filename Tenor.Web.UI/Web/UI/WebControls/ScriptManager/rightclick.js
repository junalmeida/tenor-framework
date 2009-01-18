//Disable right click script 
//visit http://www.rainbow.arch.scriptmania.com/scripts/ 
var message="Sorry, right-click has been disabled"; 
/////////////////////////////////// 

function checkObj(e) {
    var obj;
    if (e && e.target) obj = e.target; 
    if (e && e.srcElement) obj = e.srcElement; 
    
    if (obj) {
        switch (obj.tagName.toLowerCase()) {
            case "textarea":
            case "input":
            case "select":
                return true;
        }
    }
    return false;
}
function clickIE() {
    if (document.all) {(message);return false;}
} 
function keyNS(e) {
    if (document.layers||(document.getElementById&&!document.all)) { 
        if (checkObj(e)) return true;
        if (e.ctrlKey) {
            (message);
            return false;
        }
    }
}
function clickNS(e) {
    if (document.layers||(document.getElementById&&!document.all)) { 
        if (checkObj(e)) return true;
        if (e.which==2||e.which==3) {
            (message);
            return false;
        }
    }
} 
if (document.layers) {
    document.captureEvents(Event.MOUSEDOWN);
    document.onmousedown=clickNS;
    document.captureEvents(Event.KEYDOWN);
    document.onkeydown=keyNS;
} else { 
    document.onmouseup=clickNS;
    document.oncontextmenu=clickIE;
    document.onkeydown=keyNS;
} 
document.oncontextmenu=new Function("return false") 
// --> 
