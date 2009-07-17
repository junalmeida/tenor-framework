// ReorderButton

function ReorderButton(gridViewId) {
	// private variables
	this.gridViewId = gridViewId;
	this.gridview = document.getElementById(gridViewId);
	if (!this.gridview) throw "Invalid gridView id";
	this.hidden = document.getElementById(gridViewId + "_order");
	if (!this.hidden) throw "Invalid gridView hidden field id";
	
	// privileged variables
    this.animObj = null;
}


function ReorderButton_findPos(obj) {
	var curleft = curtop = 0;
	if (obj.offsetParent) {
		curleft = obj.offsetLeft
		curtop = obj.offsetTop
		while (obj = obj.offsetParent) {
			curleft += obj.offsetLeft
			curtop += obj.offsetTop
		}
	}
	return [curleft,curtop];
}

ReorderButton.prototype.doAnim = function (rowFrom, rowTo, table) {
    var rowPos = ReorderButton_findPos(rowFrom);
    var rowPos2 = ReorderButton_findPos(rowTo);
    var fromPx = rowPos[1];
    var toPx = rowPos2[1];
    
    
    this.animObj = document.createElement("div");
    this.animObj.disable=true;
    this.animObj.id="ReorderButton_" & this.gridViewId & "_AnimDiv";
    document.body.appendChild(this.animObj);
    try {
        if (table) {
            if (table.outerHTML) {
                    this.animObj.innerHTML = table.outerHTML;
            } else {
                this.animObj.appendChild(table);
            }
        }
    } catch (ex) {
    }
    
/*    this.animObj.style.border = "dotted 1px black";*/
    this.animObj.style.position = "absolute";
    this.animObj.style.left = rowPos[0].toString() + "px";
    this.animObj.style.top = rowPos[1].toString() + "px";
    
    this.animObj.style.width = rowTo.offsetWidth.toString() + "px";
    this.animObj.style.height = rowTo.offsetHeight.toString()  + "px";
    
    setTimeout("ReorderButton_Anim('" + this.gridViewId + "', " + fromPx.toString() + ", " + toPx.toString() + ", 5);", 5);
    
}

function ReorderButton_Anim(gridViewId, fromPx, toPx, time) {
    var obj = document.getElementById("ReorderButton_" & gridViewId & "_AnimDiv");
    if (obj) {
        var changeTime = 0;
        var alpha = 100;
        
        var rowPos = ReorderButton_findPos(obj);
        if (toPx > fromPx) {
            rowPos[1]+=3;
            if (rowPos[1] > toPx) rowPos[1] = toPx;
            
            
            if (rowPos[1] > toPx - ((toPx-fromPx)/2)) {
                alpha = (100- ((100*(rowPos[1]-fromPx)) / (toPx-fromPx)))*2;
                changeTime = 10 - ((5*(rowPos[1]-fromPx)) / (toPx-fromPx));
            }
          
        } else {
            rowPos[1]-=3;
            if (rowPos[1] < toPx) rowPos[1] = toPx;
            
            if (rowPos[1] < toPx + ((fromPx-toPx)/2)) {
                alpha = ((100*(rowPos[1]-toPx)) / (fromPx-toPx)) *2;
                changeTime = 10 - ((5*(rowPos[1]-toPx)) / (fromPx-toPx));
            }
        }
        
        
        time+=changeTime;

        alpha=Math.round(alpha);
        ReorderButton_SetAlpha(obj, alpha);
        
        obj.style.top = (rowPos[1]).toString() + "px";
        if (rowPos[1]==toPx) {
            obj.style.display = "none";
            document.body.removeChild(obj);
        } else {
            setTimeout("ReorderButton_Anim('" + gridViewId + "', " + fromPx.toString() + ", " + toPx.toString() + ", " + time.toString() + ")", time);
        }
        
    }
}

function ReorderButton_SetAlpha(obj, value) {
    try {
        obj.style.filter = "Alpha(Opacity=" + value.toString() + ")";  
    } catch (e) {} 
    try {
        obj.style.mozOpacity = (value / 100).toString();
    } catch (e) {} 
    try {
        obj.style.opacity = (value / 100).toString();
    } catch (e) {} 

}

ReorderButton.prototype.arrOrdem = [];


ReorderButton.prototype.ReOrder = function(btn, id, dir) {

        var firstRow = 1;
        var row = btn.parentNode.parentNode;
        
        var tbl = document.getElementById(this.gridViewId);
        
        if(row != undefined && tbl != undefined)
        {
            var indiceRow = 0;
            for(var i = 0; i < tbl.rows.length; i++)
            {
                if(tbl.rows[i] == row)
                {
                    indiceRow = i;
                    break;
                }
            }
            
            if (indiceRow<firstRow) return;
            
            var modificador = 0;
            if(dir == "up" && indiceRow > firstRow)
            {
                modificador = -1;

            }
            else if(dir == "down" && indiceRow < (tbl.rows.length - 1))
            {
                modificador = +1;
            }
            
            if(modificador != 0)
            {
                //we have to iterate cell by cell. ie bug?
                
                var row = tbl.insertRow(tbl.rows.length);
                row.style.display="none";
                //tries to keep classname.
                row.className = tbl.rows[indiceRow].className;
                for (i in tbl.rows[indiceRow].style) {
                    try {
                        row.style[i] = tbl.rows[indiceRow].style[i];
                    } catch (e) {}
                }
                
                // holds the data of the first item
                for(var j = 0; j < tbl.rows[indiceRow].cells.length; j++)
                {
                    var cell = row.insertCell(j);
                    //tries to keep classname.
                    cell.className = tbl.rows[indiceRow].cells[j].className;
                    for (i in tbl.rows[indiceRow].cells[j].style) {
                        try {
                            cell.style[i] = tbl.rows[indiceRow].cells[j].style[i];
                        } catch (e) {}
                    }
                    cell.style.width = tbl.rows[indiceRow].cells[j].offsetWidth.toString() + "px";
                    cell.style.height = tbl.rows[indiceRow].cells[j].offsetHeight.toString() + "px";
                    //--
                    
                    cell.innerHTML = tbl.rows[indiceRow].cells[j].innerHTML;
                }
                // sets the second one data to the first one.
                for(var j = 0; j < tbl.rows[indiceRow].cells.length; j++)
                {
                    tbl.rows[indiceRow].cells[j].innerHTML = tbl.rows[indiceRow + modificador].cells[j].innerHTML;
                }
                // sets the first one data to the second one.
                for(var j = 0; j < tbl.rows[indiceRow + modificador].cells.length; j++)
                {
                    tbl.rows[indiceRow + modificador].cells[j].innerHTML = row.cells[j].innerHTML;
                }
                
                var table = document.createElement("table");
                for (i in tbl.style) {
                    try {
                        table.style[i] = tbl.style[i];
                    } catch (e) {}
                }
                var newrow = row.cloneNode(true);
                newrow.style.display="";
                table.appendChild(newrow);
            
                // clearing buffer row
                tbl.deleteRow(tbl.rows.length - 1);
                
                
                for(var i = 0; i < this.arrOrdem.length; i++)
                {
                    if(this.arrOrdem[i] == id)
                    {
                        var id1 = this.arrOrdem[i];
                        var id2 = this.arrOrdem[i + modificador];
                        this.arrOrdem[i] = id2;
                        this.arrOrdem[i + modificador] = id1;
                        break;
                    }
                }
                
                
/*                
                table.className = tbl.className;
                for (i in tbl.style) {
                    try {
                        table.style[i] = tbl.style[i];
                    } catch (e) {}
                }
*/                
                this.doAnim(tbl.rows[indiceRow], tbl.rows[indiceRow + modificador], table);
            }
        }
        

        
        this.hidden.value = this.arrOrdem;

        return false;
    }
