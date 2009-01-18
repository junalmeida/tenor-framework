var SortableOrder = new Class({
 
	options: {
		handles: false,
		onStart: Class.empty,
		onComplete: Class.empty,
		ghost: true,
		snap: 3,
		onDragStart: function(element, ghost){
			ghost.setStyle('opacity', 0.5);
		},
		onDragComplete: function(element, ghost){
			ghost.remove();
		}
	},
 
	initialize: function(list, options){
		this.setOptions(options);
		this.list = $(list);
		this.elements = this.list.getChildren();
		this.handles = $$(this.options.handles) || this.elements;
		this.drag = [];
		this.bound = {'start': []};
 
		this.positions = [];
 
		this.elements.each(function(el, i){
 
			el.position = i;
			el.dropIn = null;
			el.addEvents({
				over: function(draggedElement){
					draggedElement.dropIn = this;
					var debug = $('debug')
					if (debug) debug.innerHTML += " over this:" + this.position;
				}
			});
 
			var droppables = $A(this.elements);
			droppables.remove(el);
 
			this.trash = new Element('div').injectInside(document.body);
			var limit = this.list.getCoordinates();
 
			this.drag[i] = new Drag.Move(el, {
				handle: this.handles[i],
				snap: false,
				droppables: droppables,
				onBeforeStart: function(element, event){
					var offsets = element.getPosition();
					this.old = element;
					this.drag[i].element = this.ghost = element.clone().setStyles({
						'position': 'absolute',
						'top': offsets.y + 'px',
						'left': offsets.x + 'px'
					}).injectInside(this.trash);
					element.setStyle('visibility', 'hidden');
					this.fireEvent('onDragStart', [el, this.ghost]);						
				}.bind(this),
				onComplete: function(element){
					this.old.setStyle('visibility', 'visible');
					this.drag[i].element = this.old;						
					this.fireEvent('onDragComplete', [el, this.ghost]);
					this.fireEvent('onComplete');
				}.bind(this),
 
				onDrag: function(draggedElement){
					var el = this.old;
					if (draggedElement.dropIn != null && draggedElement.dropIn.position != el.position){
 
						if (el.position < draggedElement.dropIn.position){
							(draggedElement.dropIn.getNext())? el.parentNode.insertBefore(el, draggedElement.dropIn.getNext()) : el.parentNode.insertBefore(el, null);
						} else if (el.position > draggedElement.dropIn.position) {
							el.parentNode.insertBefore(el, draggedElement.dropIn);
						}
 
						this.updateOrder(el.position, draggedElement.dropIn.position);
						draggedElement.dropIn = null;
					}
 
				}.bind(this)
 
			});
 
		}, this);
		if (this.options.initialize) this.options.initialize.call(this);
 
		this.positions.each(function(val, key){ alert( key + ' ' + val); })
 
	},
 
	updateOrder: function(fromPosition, toPosition){
		this.elements.each(function(el){
			if (el.position == fromPosition){
				 el.position = toPosition;
			} else if (el.position > fromPosition && el.position <= toPosition){
				el.position--;
			} else if (el.position < fromPosition && el.position >= toPosition){
				el.position++;
			}
		});
	},
 
	detach: function(){
		this.elements.each(function(el, i){
			this.handles[i].removeEvent('mousedown', this.bound.start[i]);
		}, this);
	},
 
	serialize: function(){
		var serial = [];
		this.list.getChildren().each(function(el, i){
			serial[i] = this.elements.indexOf(el);
		}, this);
		return serial;
	},
 
	end: function(el){
		document.removeEvent('mousemove', this.bound.move);
		document.removeEvent('mouseup', this.bound.end);
		this.fireEvent('onComplete', el);
	}
 
});
SortableOrder.implement(new Events);
SortableOrder.implement(new Options);    
