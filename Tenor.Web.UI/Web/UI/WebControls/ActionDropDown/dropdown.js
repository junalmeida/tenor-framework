// DropDownList 
// Copyright (c) 2006 Marcos Almeida Jr.()

var _ActionDropDownList_Ids = new Array();
var _ActionDropDownList_Indexes = new Array();

function ActionDropDownList_Check(obj, msg) {
    var idindex = -1;
    for (var i=0; i<_ActionDropDownList_Ids.length;i++) {
        if (_ActionDropDownList_Ids[i]==obj.id)
            idindex=i;
    }
    if (idindex == -1) {
        idindex = _ActionDropDownList_Ids.push(obj.id) - 1; //retorna o novo length -1
        if (obj.options.length > 0)
            _ActionDropDownList_Indexes.push(0);
        else
            _ActionDropDownList_Indexes.push(-1);
    }
    
    if (obj.options[obj.selectedIndex].id == obj.id + "_AddNewItem") {
        obj.selectedIndex=_ActionDropDownList_Indexes[idindex];
        //NewItem
        var res = prompt(msg, "");
        if (res) {
            __doPostBack(obj.name,res);
        }
    }
    
    _ActionDropDownList_Indexes[idindex]=obj.selectedIndex;
}