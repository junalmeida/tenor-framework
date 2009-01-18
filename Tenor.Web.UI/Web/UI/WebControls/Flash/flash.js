/*theObjects = document.getElementsByTagName("object");
for (var i = 0; i < theObjects.length; i++) {
theObjects[i].outerHTML = theObjects[i].outerHTML;
}*/

function writeFlash(id, width, height, src, bgcolor, wmode, flashvars, scriptAccess, allowFullscreen, scale, align)
{
	document.write('<object codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" height="' + height + '" width="' + width + '" classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" viewastext>');
	document.write('<param name="movie" value="' + src + '">');
	document.write('<param name="swliveconnect" value="true">');
	document.write('<param name="bgcolor" value="' + bgcolor + '" />');
	document.write('<param name="flashVars" value="' + flashvars + '" />');
	document.write('<param name="allowFullscreen" value="' + allowFullscreen + '" />');
	if (scale) {
	    document.write('<param name="scale" value="' + scale + '" />');
	}
	if (align) {
	    document.write('<param name="salign" value="' + align + '" />');
	}
	var embed = '<embed src="' + src + '" ';
	
	if (scriptAccess) {
	    document.write('<param name="allowScriptAccess" value="' + scriptAccess + '" />');
	    embed += 'allowScriptAccess="' + scriptAccess + '" ';
    }
	if(wmode != '')
	{
    	document.write('<param name="wmode" value="' + wmode + '">');
	    embed += 'wmode="' + wmode + '" ';
	}
    embed += 'quality="high" bgcolor="' + bgcolor  +  '" width="' + width + '" height="' + height + '" flashvars="' + flashvars + '" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" swliveconnect="true"';
	if (scale) {
	    embed+=' scale="' + scale + '"';
	}
	if (align) {
	    embed+=' salign="' + align + '"';
	}
    embed += '></embed>';
	document.write(embed);
	document.write('</object>');
}