function SortableBulletedList(id, values, orientation, runOnComplete, idprefix) {
    if (orientation && orientation == 'horizontal') {
	    new SortableOrder($(id), {
		    ghost: true,	
		    handles: $$(id.id + ' li'),		
		    onComplete: runOnComplete
	    });
    } else {
        new Sortables($(id), {
	        initialize: function(){
	            this.runOnComplete = runOnComplete;
    	    
	            values.each(function(item, i){
                   this.elements[i].valor = item;
                }, this);
	        }
	        ,
	        onStart: function(){
    	    
	        }
	        ,
	        onComplete: function(){
	            var objHidden = $(idprefix + this.list.id);
	            objHidden.value = "";
	            for (var i = 0;i < this.list.childNodes.length; i++) {
	                var obj = this.list.childNodes[i];
	                if (obj.tagName && obj.tagName == "LI") {
	                    objHidden.value += "," + obj.valor;
	                }
	            }
    	        
	            if (this.runOnComplete) {
                    this.runOnComplete();
	            }
	        }
    	    
        });
    }
}

