using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				public class CustomStyle : Style
				{
					public CustomStyle()
					{
						_css = new System.Web.UI.HtmlControls.HtmlGenericControl("div").Style;
						
					}
					
					
					private CssStyleCollection _css;
					public CssStyleCollection Style
					{
						get
						{
							return _css;
						}
					}
					
					public override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer, System.Web.UI.WebControls.WebControl owner)
					{
						base.AddAttributesToRender(writer, owner);
						foreach (string key in Style.Keys)
						{
							writer.AddStyleAttribute(key, Style[key]);
						}
					}
					
					protected override void FillStyleAttributes(System.Web.UI.CssStyleCollection attributes, System.Web.UI.IUrlResolutionService urlResolver)
					{
						base.FillStyleAttributes(attributes, urlResolver);
						foreach (string key in Style.Keys)
						{
							attributes[key] = Style[key];
						}
					}
				}
			}
		}
	}
	
}
