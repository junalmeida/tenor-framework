// CheckBoxList 
// Copyright (c) 2006 Marcos Almeida Jr.()

function CheckBoxList_SetAll(Id, Count, Value)  {
    for (var i=0;i<Count;i++) {
        var obj = document.getElementById(Id + "_" + i.toString());
        if (obj) {
            obj.checked = Value;
        }
    }
}