//Menu

var __Menus = new Array();

function Menu_FindTD(node) {
    node = node.parentElement;
    if (!node) {
        return null;
    }
    
    if (node.tagName == "TD" && node.onmouseover) {
        return node;
    } else {
        return Menu_FindTD(node);
    }
}

function Menu_FindTR(node) {
    node = node.parentElement;
    if (!node) {
        return null;
    }
    
    if (node.tagName == "TR" && node.onmouseover) {
        return node;
    } else {
        return Menu_FindTR(node);
    }
}


function Menu_CreateIframe(objpos) {
    if (document.createElement) {
        var iframe = document.createElement("iframe");
        iframe.src = "";
        iframe.frameborder = 0;
        iframe.style.position = "absolute";
        iframe.style.left = objpos.x + "px";
        iframe.style.top = objpos.y + "px";
        iframe.style.width = objpos.width + "px";
        iframe.style.height = objpos.height + "px";

        iframe.style.zIndex = 100000;
        iframe.style.visibility = "visible";
        iframe.style.filter = "Alpha(opacity=0)";
        if (iframe.style.filters) {
            iframe.style.filters.item("Alpha").Opacity = 0;
        }
        
        document.body.appendChild(iframe);

        return iframe;
        
    }
}

function Menu_CheckUnhide() {
    if (__Menus.length > 0) {
        for(var i =0; i<__Menus.length;i++) {
            if (__Menus[i].frm && __Menus[i].obj) {
                __Menus[i].frm.style.visibility=__Menus[i].obj.style.visibility;
            }

            if (__Menus[i].obj.style.visibility=="hidden" && __Menus[i].root) {
                    var node = (__Menus[i].root.tagName.toLowerCase() == "td") ?
                        __Menus[i].root:
                        __Menus[i].root.cells[0];
                    var nodeTable = WebForm_GetElementByTagName(node, "table");
                    if (nodeTable.hoverClass) {
                        WebForm_RemoveClassName(nodeTable, nodeTable.hoverClass);
                    }
                    node = nodeTable.rows[0].cells[0].childNodes[0];
                    if (node.hoverHyperLinkClass) {
                        WebForm_RemoveClassName(node, node.hoverHyperLinkClass);
                    }
            }
        }
    }
    setTimeout("Menu_CheckUnhide()", 15);
}

function Menu_Manipulate(item, isroot) {
    var node = (item.tagName.toLowerCase() == "td") ?
        item:
        item.cells[0];
    var nodeTable = WebForm_GetElementByTagName(node, "table");
    node = nodeTable.rows[0].cells[0].childNodes[0];
 
  
  var child = Menu_FindSubMenu(node);
  
  
  if (child) {
    if (document.all) {
        Menu_RescanDynamic(child.firstChild);
    } else { 
        Menu_RescanDynamic(child);
    }
    
    var doCreateItem = -1;
    if (__Menus.length > 0) {
        for (var i =0;i<__Menus.length;i++) {
            if (__Menus[i].obj == child) doCreateItem = i;
            if (__Menus[i].frm)
                __Menus[i].frm.style.visibility=__Menus[i].obj.style.visibility;
        }
    }
    var objpos;
    if (doCreateItem==-1) {
        objpos = WebForm_GetElementPosition(child);
        var it = new Object();
        it.obj = child;
          if (document.all) {
                var frm = Menu_CreateIframe(objpos);
                it.frm = frm;
          }
        if (isroot) {
            it.root = item;
        } else  {
            it.root = null;
        }
        it.originalpos = objpos;
        __Menus.push(it);
    } else {
        objpos = __Menus[doCreateItem].originalpos;
    }
        
    if (isroot) {
        Menu_HoverRoot(item);

    
        var top = new Number(child.style.top.replace(/px/ig, ""));
        child.style.top = (top - 1) + "px";
        if (child.parentElement) {
            child.parentElement.style.top=(top - 1) + "px";
            child.parentElement.style.filter = "Alpha(opacity=0)";
        }  
    } else {
        if (document.all)
            WebForm_SetElementX(child, objpos.x - 3);
    }
      
    child.style.zIndex = 100001;
        
        
      
      
  } else {
    /*alert("child not found");*/
  }

} 



function Menu_ManipulateDynamic(item) {
    var node = (item.tagName.toLowerCase() == "tr") ?
        item:
        item.rows[0];
    var nodeTable = WebForm_GetElementByTagName(node, "table");
    node = nodeTable.rows[0].cells[0].childNodes[0];
 
  /*var node = Menu_HoverRoot(item);*/
  
  var child = Menu_FindSubMenu(node);
  
  if (child) {
    child.style.zIndex = 100001;
  }
} 





function Menu_Rescan(clientid) {
    var tb = document.getElementById(clientid);
    var tds = tb.getElementsByTagName("td");
    for (var i = 0; i < tds.length; i++) {
      if (tds[i].onmouseover) {
          if (tds[i].addEventListener){
            tds[i].addEventListener("mouseover", function() {Menu_Manipulate(this, true);}, false); 
          } else if (tds[i].attachEvent){
            tds[i].attachEvent("onmouseover",  function() {   Menu_Manipulate(Menu_FindTD(event.srcElement), true);  });
          }
      } 
    } 
    Menu_CheckUnhide();
}

function Menu_RescanDynamic(divroot) {
    if (divroot) {
        var trs = divroot.getElementsByTagName("tr");
    
        for (var i = 0; i < trs.length; i++) {
          if (trs[i].onmouseover) {
            if (trs[i].addEventListener){
                trs[i].addEventListener("mouseover", function() {Menu_ManipulateDynamic(this);}, false); 
            } else if (trs[i].attachEvent){
               trs[i].attachEvent("onmouseover",  function() {   Menu_Manipulate(Menu_FindTR(event.srcElement), false);  });
            }

          
          } 
        } 
    }
}

