/* Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor.BLL
{
    internal static class Util
    {


        /// <summary>
        /// Gets a value of a property parsing the expression on a C# style.
        /// </summary>
        /// <param name="expression">The expression to find a property. Can have multiple properties separated by a dot and array values on brackets.</param>
        /// <param name="instance">The instance to use on getting the value.</param>
        /// <returns>The value to the property.</returns>
        /// <example>
        /// <code>object someObject = Util.GetValue("People[0].Name", someInstance);</code>
        /// </example>
        internal static object GetValue(string expression, object instance)
        {
            List<string> properties = new List<string>(expression.Split('.'));
            if (properties.Count == 0)
            {
                throw (new ArgumentException("Invalid expression", "expression", null));
            }
            else if (string.IsNullOrEmpty(properties[0].Trim()))
            {
                throw (new ArgumentException("Invalid expression", "expression", null));
            }
            else if (instance == null)
            {
                throw (new NullReferenceException("Invalid instance on Expression \'" + expression + "\'"));
            }
            else
            {
                object currentObject = null;
                if (properties[0].EndsWith("]"))
                {
                    //we have an array
                    int pos = properties[0].IndexOf('[');
                    if (pos > 0)
                    {
                        string propName = properties[0].Substring(0, pos);
                        string strPropIndex = properties[0].Substring(pos + 1, properties[0].Length - pos - 2);
                        int propIndex = -1;

                        if (int.TryParse(strPropIndex, out propIndex))
                        {
                            System.Reflection.PropertyInfo propInfo = instance.GetType().GetProperty(propName);

                            if (propInfo == null)
                            {
                                throw (new ArgumentException("Property \'" + propName + "\' not found on \'" + instance.GetType().FullName + "\'", "expression", null));
                            }
                            else if (propInfo.PropertyType.IsArray)
                            {
                                currentObject = propInfo.GetValue(instance, new object[] { propIndex });
                            }
                            else
                            {
                                currentObject = propInfo.GetValue(instance, null);
                                if (currentObject.GetType().GetInterface(typeof(IList).FullName) != null)
                                {
                                    currentObject = typeof(IList).GetProperty("Item").GetValue(currentObject, new object[] { propIndex });
                                }
                                else if (currentObject.GetType().GetInterface(typeof(IList<>).FullName) != null)
                                {
                                    currentObject = currentObject.GetType().GetInterface(typeof(IList<>).FullName).GetProperty("Item").GetValue(currentObject, new object[] { propIndex });
                                }
                                else
                                {
                                    throw (new ArgumentException("Cannot index type", "expression", null));
                                }
                            }
                        }
                        else if (strPropIndex == "*")
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            throw (new ArgumentException("Invalid syntax", "expression", null));
                        }

                    }
                    else
                    {
                        throw (new ArgumentException("Invalid syntax", "expression", null));
                    }
                }
                else
                {
                    System.Reflection.PropertyInfo propinfo = instance.GetType().GetProperty(properties[0]);
                    if (propinfo == null)
                    {
                        throw (new ArgumentException("Property \'" + properties[0] + "\' not found on \'" + instance.GetType().FullName + "\'", "Expression", null));
                    }
                    else
                    {
                        currentObject = propinfo.GetValue(instance, null);
                    }
                }


                if (properties.Count > 1)
                {
                    properties.RemoveAt(0);
                    currentObject = GetValue(string.Join(".", properties.ToArray()), currentObject);
                }
                return currentObject;
            }
        }

        internal static int Compare(BLLBase x, BLLBase y, string propertyExpression)
        {
            string[] props = (propertyExpression + ",").Split(',');
            string _PropertyExpression = "";

            int lastIndex = -1;
            for (int i = 0; i <= props.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(props[i]))
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
                    return -1;
                }
                else
                {
                    if (lastIndex + 1 < props.Length && !string.IsNullOrEmpty(props[lastIndex + 1]))
                    {
                        string newProperties = "";
                        for (int i = lastIndex + 1; i <= props.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(props[i]))
                            {
                                newProperties += "," + props[i];
                            }
                        }
                        if (newProperties != "")
                        {
                            return Compare(x, y, newProperties.Substring(1));
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
                throw (new ArgumentException("The type of the Property supplied is not comparable.", "propertyExpression", null));
            }

        }

        internal static System.Web.HttpContext Context
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

        internal static System.Web.SessionState.HttpSessionState Session
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

        internal static System.Web.HttpRequest Request
        {
            get
            {
                return Context.Request;
            }
        }

        internal static System.Web.HttpResponse Response
        {
            get
            {
                return Context.Response;
            }
        }
    }
}