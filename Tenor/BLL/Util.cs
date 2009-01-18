using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace BLL
	{
		internal class Util
		{
			
			
			/// <summary>
			/// Esta função retorna um valor a partir de uma expressão estilo C#
			/// </summary>
			/// <param name="Expression">Expressão para buscar o valor</param>
			/// <param name="Instance">Instancia inicial</param>
			/// <returns></returns>
			/// <remarks>Exemplos: Empreendimentos[0].MarketingEmpreendimento.Ordem</remarks>
			internal static object GetValue(string Expression, object Instance)
			{
				List<string> propriedades = new List<string>(Expression.Split('.'));
				if (propriedades.Count == 0)
				{
					throw (new ArgumentException("Invalid expression", "Expression", null));
				}
				else if (string.IsNullOrEmpty(propriedades[0].Trim()))
				{
					throw (new ArgumentException("Invalid expression", "Expression", null));
				}
				else if (Instance == null)
				{
					throw (new NullReferenceException("Invalid instance on Expression \'" + Expression + "\'"));
				}
				else
				{
					object objeto = null;
					if (propriedades[0].EndsWith("]"))
					{
						//é um array
						int pos = propriedades[0].IndexOf('[');
						if (pos > 0)
						{
							string propName = propriedades[0].Substring(0, pos);
							string strPropIndex = propriedades[0].Substring(pos + 1, propriedades[0].Length - pos - 2);
							int propIndex = - 1;
							
							if (int.TryParse(strPropIndex, out propIndex))
							{
								System.Reflection.PropertyInfo propInfo = Instance.GetType().GetProperty(propName);
								
								if (propInfo == null)
								{
									throw (new ArgumentException("Property \'" + propName + "\' not found on \'" + Instance.GetType().FullName + "\'", "Expression", null));
								}
								else if (propInfo.PropertyType.IsArray)
								{
									objeto = propInfo.GetValue(Instance, new object[] {propIndex});
								}
								else
								{
									objeto = propInfo.GetValue(Instance, null);
									if (objeto.GetType().GetInterface(typeof(IList).FullName) != null)
									{
										objeto = typeof(IList).GetProperty("Item").GetValue(objeto, new object[] {propIndex});
									}
									else if (objeto.GetType().GetInterface(typeof(IList<>).FullName) != null)
									{
										objeto = objeto.GetType().GetInterface(typeof(IList<>).FullName).GetProperty("Item").GetValue(objeto, new object[] {propIndex});
									}
									else
									{
										throw (new ArgumentException("Cannot index type", "Expression", null));
									}
								}
							}
							else if (strPropIndex == "*")
							{
								throw (new NotImplementedException("This feature is not intended to be ever implemented. <br />- The IT department."));
							}
							else
							{
								throw (new ArgumentException("Invalid syntax", "Expression", null));
							}
							
						}
						else
						{
							throw (new ArgumentException("Invalid syntax", "Expression", null));
						}
					}
					else
					{
						System.Reflection.PropertyInfo propinfo = Instance.GetType().GetProperty(propriedades[0]);
						if (propinfo == null)
						{
							throw (new ArgumentException("Property \'" + propriedades[0] + "\' not found on \'" + Instance.GetType().FullName + "\'", "Expression", null));
						}
						else
						{
							objeto = propinfo.GetValue(Instance, null);
						}
					}
					
					
					if (propriedades.Count > 1)
					{
						propriedades.RemoveAt(0);
						objeto = GetValue(string.Join(".", propriedades.ToArray()), objeto);
					}
					return objeto;
				}
			}
			
			internal static int Compare(BLLBase x, BLLBase y, string PropertyExpression)
			{
				string[] props = (PropertyExpression + ",").Split(',');
				string _PropertyExpression = "";
				
				int lastIndex = - 1;
				for (int i = 0; i <= props.Length - 1; i++)
				{
					if (! string.IsNullOrEmpty(props[i]))
					{
						_PropertyExpression = props[i].Trim();
						lastIndex = i;
						break;
					}
				}
				
				
				
				object v1 = Util.GetValue(_PropertyExpression, x);
				object v2 = Util.GetValue(_PropertyExpression, y);
				
				
				if (v1.GetType() == typeof(string))
				{
					
					return string.Compare(v1.ToString(), v2.ToString());
				}
				else if (v1.GetType().IsValueType && v1.GetType().IsPrimitive)
				{
					
					double dv1 = Convert.ToDouble(v1);
					double dv2 = Convert.ToDouble(v2);
					
					if (dv1 > dv2)
					{
						return 1;
					}
					else if (dv1 < dv2)
					{
						return - 1;
					}
					else
					{
						if (lastIndex + 1 < props.Length && ! string.IsNullOrEmpty(props[lastIndex + 1]))
						{
							string novosProps = "";
							for (int i = lastIndex + 1; i <= props.Length - 1; i++)
							{
								if (! string.IsNullOrEmpty(props[i]))
								{
									novosProps += "," + props[i];
								}
							}
							if (novosProps != "")
							{
								return Compare(x, y, novosProps.Substring(1));
							}
							else
							{
								return 0;
							}
						}
						else
						{
							
							return 0;
						}
					}
				}
				else
				{
					throw (new ArgumentException("The type of the Property supplied is not comparable", "PropertyExpression", null));
				}
				
			}
			
			
			
			
			
			
			
			
			public static System.Web.HttpContext Context
			{
				get
				{
					System.Web.HttpContext ctx = System.Web.HttpContext.Current;
					if (ctx == null)
					{
						throw (new InvalidOperationException("Cannot find an HttpContext. This function can only be used with ASP.NET"));
					}
					return ctx;
				}
			}
			
			public static System.Web.SessionState.HttpSessionState Session
			{
				get
				{
					System.Web.SessionState.HttpSessionState sess = Context.Session;
					if (sess == null)
					{
						throw (new InvalidOperationException("Session state is not available."));
					}
					return sess;
				}
			}
			
			public static System.Web.HttpRequest Request
			{
				get
				{
					return Context.Request;
				}
			}
			
			public static System.Web.HttpResponse Response
			{
				get
				{
					return Context.Response;
				}
			}
		}
	}
}
