// TextBox
var TextBox_ID = "";
function TextBox_HandleChange(inst) {
    var obj = document.getElementById(TextBox_ID);
    if (obj) {
        obj.value = inst.getBody().innerHTML;
    }
}



// Masks 

function Masks_Integer(e) {
  var tecla=Masks_getKey(e);
 
  if(tecla > 47 && tecla < 58) { 
    return true;
  } else {
      if (tecla != 8 && tecla != 0) {
        return false;
      } else { 
        return true; /* Backspace */
      }
  }
  /* cross browser adapted */

}

function Masks_Float(e, obj) {
  /*  
    var tecla = 0;
    var evt = new Masks_CrossEvent(e);
    if (!document.all && evt.keyCode > 0) {
        return (true);
    } else {
        tecla = evt.charCode;
    }

  */
  var tecla=Masks_getKey(e);
  
  if(tecla > 47 && tecla < 58) {
    return true; 
  }    
  
  if(tecla == 44) {
		if(obj.value.length == 0) {
			obj.value = '0' + obj.value; /* 0, */
			return true;
		} else {
			if(obj.value.indexOf(",") != -1) {  
				return false; /* permite somente uma , */
			} else {
				return true;
			}
		}
  } else if (tecla==0 || tecla == 8) {
        return true;		  
  } else {
		return false;  
  }
  
  /* cross browser adapted */
}

function Masks_FormatFloat(obj, numCasasDecimais)
{
  var i;
  var dif;
  if (obj.value.length == 0)
	return false;	
  var strfloat = new Number(new String(obj.value).replace(/,/i, "."));
  if (isNaN(strfloat)) {
    obj.value = "";
  } else {
    var strres = new String(strfloat.toFixed(2));
    obj.value = strres.replace(/\./g, ",");
  }
  /* cross browser adapted */
 		
}


function Masks_Currency(e, myField) 
{
  /* cross browser adapted */
  
    var tecla=Masks_getKeyCode(e);

	if(tecla == 9 || tecla == 13) // If TAB or ENTER, returns TAB
	{
		return true;
	}
	
	var pos_virgula;
	var a ;
	var b ;
	var c ;
	var d ;
	var valor_temp ;


	valor_temp = myField.value ;
	if (valor_temp=="") valor_temp="0,00";
	
	pos_virgula = valor_temp.indexOf(',',0); 
	
	//avoid "BackSpace" 
	//////////////////////////////////////////////////////////
	if (tecla == 8)
		{
		 var num;

		 var tamanho = valor_temp.length;

		
		 if (tamanho > 4)	
			  a = valor_temp.substr( 0 , pos_virgula -1 )+ "," ;
   
		 if ((tamanho < 5))	
			  a =  "0," ;

		  b = valor_temp.substr( pos_virgula - 1 , 1 ) ;
		  c = valor_temp.substr( pos_virgula + 1 , 1 )  ;
		  
		 valor_temp = a + b +  c ;

		}
	//////////////////////////////////////////////////////////


	//writes numbers
	//////////////////////////////////////////////////////////
	if(tecla > 95 && tecla < 106) // numeros de 0 a 9
	{
		tecla = tecla - 48;
	}
	if(tecla > 47 && tecla < 58) // numeros de 0 a 9
		{
		var num;
		var j;
		j=valor_temp.indexOf(',',0); 
   
		a = valor_temp.substr(0,j)  ;
		if (a=='0')
			{
			 a='';
			}
		b = valor_temp.substr(j+1,1) + "," ;
		c = valor_temp.substr(j+2,1)  ;
		d = (tecla - 48) ;
		valor_temp = a + b +  c +  d;

   }   

	if (valor_temp.length > 6)
		{
		 var parte_decimal ;
		 var parte_inteira ;
		 var parte_colocar_pontos ='' ;
		 var i ;
		 var resto ='' ;
		 var cont = 0 ;
		 
		 pos_virgula = valor_temp.indexOf(',',0); 

	     parte_decimal = valor_temp.substr( pos_virgula , 3 ) ;
	     parte_inteira = valor_temp.substr( 0 , pos_virgula ) ;
	     
	     //Remove dots 
	     //-------------------------------------------------------------
	     for (i= parte_inteira.length   ; i >-1  ; i--) 
			{
	     	 parte_inteira = parte_inteira.replace('.','');
	     	}
	     //-------------------------------------------------------------


	     //Writes dots
	     //-------------------------------------------------------------
          for (i= parte_inteira.length   ; i >-1  ; i--) 
			{
			 parte_colocar_pontos =  parte_inteira.substr( i , 1 ) + parte_colocar_pontos ;
			 resto = cont % 3;
			 if ( resto == 0 && cont !=0 && cont != parte_inteira.length)
				{
				 parte_colocar_pontos =  '.' + parte_colocar_pontos   ;
				}
				cont = cont + 1 ;
			}
		 //-------------------------------------------------------------

		  valor_temp = parte_colocar_pontos + parte_decimal ;
	     
		}
	
    myField.value = valor_temp ;
    
	//movers the cursor to end
	//////////////////////////////////////////////////////////
	if (document.selection) 
		{
		 myField.focus();
		 sel = document.selection.createRange();
		 sel.moveStart('character',myField.value.length + 100);
		 sel.moveEnd('character',myField.value.length + 100);
		 sel.select();
		}
		
		
	 return false;
}

function Masks_checkValue(obj, test, msg) {
    var value = new String(obj.value);
    var re = new RegExp(test, "g");
    if (!value.match(re)) {
        obj.value = "";
        if (msg) alert(msg);
    }
}

function Masks_getFloat(value) {
    value = value.toString();
    var atual = new String(value.replace(/\./ig, "").replace(/,/ig, "."));
    var res = new Number(atual);
    return res;
}

function Masks_getBrFloat(value) {
    value = value.toString();
    var atual = new String(value.replace(/,/ig, "").replace(/\./ig, ","));
    var res = new Number(atual);
    return res;
}


function Masks_Format(e, src, mask) 
{
  var i = src.value.length;
  var saida = mask.substring(0,1);
  var texto = mask.substring(i);
  
  var tecla=Masks_getKey(e);
    
if (tecla == 8 || tecla == 0) 
	return true;//  backspace 
else if (i >= mask.length)
    return false;
    	
if (tecla < 48 || tecla >=58) 
    return false; // numbers from 0 to 9 or "/" 
   
if (texto.substring(0,1) != saida) 
  {
	src.value += texto.substring(0,1);
  }
  
  /* cross browser adapted */
}



/* -------------------------------- */

function Masks_CrossEvent(evt)
{
	evt = evt? evt: (window.event? window.event: null);
	if (evt)
	{
		this.originalEvent = evt;
		this.type = evt.type;
		this.screenX = evt.screenX;
		this.screenY = evt.screenY;

		/* IE: srcElement */
		this.target = evt.target? evt.target: evt.srcElement;

		/* N4: modifiers */
		if (evt.modifiers)
		{
			this.altKey   = evt.modifiers & Event.ALT_MASK;
			this.ctrlKey  = evt.modifiers & Event.CONTROL_MASK;
			this.shiftKey = evt.modifiers & Event.SHIFT_MASK;
			this.metaKey  = evt.modifiers & Event.META_MASK;
		}
		else
		{
			this.altKey   = evt.altKey;
			this.ctrlKey  = evt.ctrlKey;
			this.shiftKey = evt.shiftKey;
			this.metaKey  = evt.metaKey;
		}

		/* N4: which // N6+: charCode */
		this.charCode = !isNaN(evt.charCode)? evt.charCode: !isNaN(evt.keyCode)? evt.keyCode: evt.which;
		this.keyCode = !isNaN(evt.keyCode)? evt.keyCode: evt.which;
		this.button = !isNaN(evt.button)? evt.button: !isNaN(evt.which)? evt.which-1: null;
		this.debug = "c:" + evt.charCode + " k:" + evt.keyCode
			+ " b:" + evt.button + " w:" + evt.which;
	}
} 

/*
function inspectObject(obj) {
	var result = "";
	var objName = obj.name? obj.name: "object";
	for (var i in obj) {
		result += objName + "." + i + " = " + obj[i] + "\n";
	}
	return result;
} 
*/
function Masks_getKeyCode(pEvent) {
   	var evt = new Masks_CrossEvent(pEvent);
    return evt.keyCode;
}


function Masks_getKey(pEvent) {
   	var evt = new Masks_CrossEvent(pEvent);
   	if (document.all) {
   	    return evt.keyCode;
   	} else {
   	    return evt.charCode;
   	}
}



    function Masks_checkCPF(sender, args) {
        var cpf = new String(args.Value);
        args.IsValid = Masks_validaCPF(cpf);
     }
    function Masks_checkCNPJ(sender, args) {
        var cnpj = new String(args.Value);
        args.IsValid = Masks_validaCNPJ(cnpj);
     }

// CPF and CNPJ - brazil only

	function Masks_validaCPF(cpf) {
	    cpf = cpf.replace(/\./ig, "");
	    cpf = cpf.replace(/\-/ig, "");
		var valor = true;
		var erro = new String("");
		if (cpf.length < 11) return false;
		
		var nonNumbers = /\D/;
		if (nonNumbers.test(cpf)) return false;	
		if (cpf == "00000000000" || cpf == "11111111111" || cpf == "22222222222" || cpf == "33333333333" || cpf == "44444444444" || cpf == "55555555555" || cpf == "66666666666" || cpf == "77777777777" || cpf == "88888888888" || cpf == "99999999999"){
			  return false;
		}
		var a = [];
		var b = new Number;
		var c = 11;
		for (i=0; i<11; i++){
			a[i] = cpf.charAt(i);
			if (i < 9) b += (a[i] *  --c);
		}
		if ((x = b % 11) < 2) { a[9] = 0 } else { a[9] = 11-x }
		b = 0;
		c = 11;
		for (y=0; y<10; y++) b += (a[y] *  c--); 
		if ((x = b % 11) < 2) { a[10] = 0; } else { a[10] = 11-x; }
		if ((cpf.charAt(9) != a[9]) || (cpf.charAt(10) != a[10])){
			return false;
		}
		if (erro.length > 0){
			return false;
		}
		return true;
	}
	function Masks_validaCNPJ(cnpj) {
	    // E' necessarios preencher corretamente o numero do cnpj
		if (cnpj.length < 18) return false;
		
		//E' necessarios preencher corretamente o numero do cnpj!
		if ((cnpj.charAt(2) != ".") || (cnpj.charAt(6) != ".") || (cnpj.charAt(10) != "/") || (cnpj.charAt(15) != "-")){
			if (erro.length == 0) return false;
		}
		//substituir os caracteres que nao sao numeros
		if(document.layers && parseInt(navigator.appVersion) == 4){
			x = cnpj.substring(0,2);
			x += cnpj.substring(3,6);
			x += cnpj.substring(7,10);
			x += cnpj.substring(11,15);
			x += cnpj.substring(16,18);
			cnpj = x;	
		} else {
			cnpj = cnpj.replace(".","");
			cnpj = cnpj.replace(".","");
			cnpj = cnpj.replace("-","");
			cnpj = cnpj.replace("/","");
		}
		var nonNumbers = /\D/;
		//A verificacao de cnpj suporta apenas numeros!
		if (nonNumbers.test(cnpj)) return false;
		var a = [];
		var b = new Number;
		var c = [6,5,4,3,2,9,8,7,6,5,4,3,2];
		for (i=0; i<12; i++){
			a[i] = cnpj.charAt(i);
			b += a[i] * c[i+1];
		}
		if ((x = b % 11) < 2) { a[12] = 0 } else { a[12] = 11-x }
		b = 0;
		for (y=0; y<13; y++) {
			b += (a[y] * c[y]); 
		}
		if ((x = b % 11) < 2) { a[13] = 0; } else { a[13] = 11-x; }
		if ((cnpj.charAt(12) != a[12]) || (cnpj.charAt(13) != a[13])){
			//Digito verificador com problema!
			return false;
		}
		/*
		if (erro.length > 0){
			alert(erro);
			return false;
		} else {
			alert("cnpj valido!");
		}
		*/
		return true;
	}
/*
function Asc(character){
    return character.charCodeAt(0);
}
*/

function __DateCheckValue(obj, msg)
{
    var value = new String(obj.value);
    if (!__ValiDATE(value))
    {
        obj.value = "";
        if (msg)
        {
            alert(msg);
        }
    }
}

// TODO: Translate this comment
// função que valida datas, para substituir a regular expression que excluía 19/02 de todos os anos (o meu aniversario!)
// adaptada do compare validator com datatype check para date do asp.net
function __ValiDATE(texto)
{
    function GetFullYear(year)
    {
        var cutoffyear = 2029;
        var twoDigitCutoffYear = cutoffyear % 100;
        var cutoffYearCentury = cutoffyear - twoDigitCutoffYear;
        return year > twoDigitCutoffYear ? cutoffYearCentury - 100 + year : cutoffYearCentury + year;
    }
    var m;
    var day, month, year;
    var yearLastExp = new RegExp("^\\s*(\\d{1,2})([-/]|\\. ?)(\\d{1,2})\\2((\\d{4})|(\\d{2}))\\s*$");
    m = texto.match(yearLastExp);
    if (m == null)
    {
        return false;
    }
    day = m[1];
    month = m[3];
    year = m[5] && m[5].length == 4 ? m[5] : GetFullYear(parseInt(m[6], 10));

    month -= 1;
    var date = new Date(year, month, day);
    if (year < 100)
    {
        date.setFullYear(year);
    }
    return typeof date == "object" &&
        year == date.getFullYear() &&
        month == date.getMonth() && day == date.getDate();
}