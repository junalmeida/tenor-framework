


//Disable select-text script (IE4+, NS6+)
//visit http://www.rainbow.arch.scriptmania.com/scripts/ 
/////////////////////////////////// 
function disableselect(e){
    e = e || window.event;
    if (e) {
        var target = e.target || e.srcElement;
        if (target && target.tagName) {
            if (target.tagName == "INPUT" || target.tagName == "TEXTAREA") 
                return true;
        }
    }


    return false;
} 
function reEnable(){
    return true;
} 
//if IE4+
if (document.all) {
    document.onselectstart=new Function ("return disableselect(event);");
}
//if NS6
if (window.sidebar){
    try {
        document.style.MozUserSelect="none";
    } catch (e) {}
    document.onmousedown=disableselect;
    document.onclick=reEnable;
}

function clearSelection () {
    if (document.selection)
        document.selection.empty();
    else if (window.getSelection)
        window.getSelection().removeAllRanges();
       
    setTimeout("clearSelection()", 100);
}
if (!document.all && !window.sidebar) {
    setTimeout("clearSelection()", 100);
}











///***********************************************
//* Disable Text Selection script- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
//* This notice MUST stay intact for legal use
//* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
//***********************************************/

/*
function disableSelection(target){
    if (target) {
        if (target.onselectstart) {
            // IE
            target.onselectstart=function(){return false;};
        } else if (target.style.MozUserSelect) {
            // Firefox
            target.style.MozUserSelect="none";
        } else {
            // outros
	        target.onmousedown=function(){return false;};
            target.style.cursor = "default";
        }
    }
}
disableSelection(document.body);
*/