// FloatingPanel Web Control
// Modificações no JS por
//   - Andre Raw
//   - Rachel Carvalho
//   - Marcos Almeida

var floatingLayers = [];

function initFloater(layerName){
    var numLayer = floatingLayers.length;
    
	floatingLayers[numLayer] = new Object();
	floatingLayers[numLayer].Layer=document.getElementById(layerName);
	floatingLayers[numLayer].defPosTop=parseInt(floatingLayers[numLayer].Layer.style.top);
	floatingLayers[numLayer].timer="";
	floatLayer(numLayer);
}

function floatLayer(numLayer){
	floatingLayers[numLayer].pageTop=(document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop);
	clearTimeout(floatingLayers[numLayer].timer);
	floatingLayers[numLayer].step=15;
	floatingLayers[numLayer].distance=(floatingLayers[numLayer].pageTop+floatingLayers[numLayer].defPosTop)-parseInt(floatingLayers[numLayer].Layer.style.top);
	floatingLayers[numLayer].divTop=parseInt(floatingLayers[numLayer].Layer.style.top);

	// ACAO SUBIR
	if(floatingLayers[numLayer].divTop<=(floatingLayers[numLayer].pageTop+floatingLayers[numLayer].defPosTop)){
		if(floatingLayers[numLayer].distance>150){
			floatingLayers[numLayer].step=20;
		}
		if(floatingLayers[numLayer].distance<20){
			floatingLayers[numLayer].step=1;
		}

		floatingLayers[numLayer].divTop+= floatingLayers[numLayer].step;
		floatingLayers[numLayer].Layer.style.top =floatingLayers[numLayer].divTop+"px";
		//alert(floatingLayers[numLayer].divTop)
	}
	//fim


	// ACAO DESCER
	if(floatingLayers[numLayer].divTop>=(floatingLayers[numLayer].pageTop+floatingLayers[numLayer].defPosTop)){
		if(floatingLayers[numLayer].distance< -150){
			floatingLayers[numLayer].step=20;
	}
			if(floatingLayers[numLayer].distance> -25){
			floatingLayers[numLayer].step=1;
		}
		floatingLayers[numLayer].divTop-= floatingLayers[numLayer].step;
		floatingLayers[numLayer].Layer.style.top =floatingLayers[numLayer].divTop+"px";
		//alert(floatingLayers[numLayer].divTop)
	}
	//fim

	floatingLayers[numLayer].timer=setTimeout("floatLayer("+numLayer+")",50);

}
//fim function

window.onscroll=function()
{
	for (i=0; i<floatingLayers.length; i++) {
		if(floatingLayers[i] != undefined)
		{
			floatLayer(i);
		}
	}
}
