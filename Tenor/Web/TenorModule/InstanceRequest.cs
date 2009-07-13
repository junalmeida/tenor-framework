using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using Tenor.Drawing;

namespace Tenor.Web
{
    public partial class TenorModule
    {

        private string _class;
        private bool FindAssembly(Assembly obj)
        {
            return (obj.GetType(_class, false, true) != null);
        }

        private bool FindProperty(PropertyInfo i)
        {
            ResponsePropertyAttribute[] atts = (ResponsePropertyAttribute[])(i.GetCustomAttributes(typeof(ResponsePropertyAttribute), true));
            if (atts.Length > 0)
            {
                return (i.PropertyType.GetInterface(typeof(IResponseObject).FullName) != null);
            }
            else
            {
                return false;
            }
        }

        private void InstanceRequest(HttpApplication app)
        {
            System.Web.Caching.Cache Cache = app.Context.Cache;
            CacheData item = null;

            object messagesLock = new object();
            if (app.Request[Tenor.Configuration.TenorModule.NoCache] == string.Empty)
            {
                lock (messagesLock)
                {
                    item = Cache.Get("instance:" + app.Request.QueryString.ToString()) as CacheData;
                }
            }

            if (item != null)
            {
                //data is already in cache for the current url
                WriteHeaders(app, item);
                WriteStream(((Stream)item.Object), app);

            }
            else
            {
                //data is not yet on cache.


                _class = QueryString("cl");

                //Hexadecimal hash - the new method
                try
                {
                    _class = Tenor.Text.Strings.FromAscHex(_class);
                }
                catch (Exception)
                {
                    try
                    {
                        //Old method, for backward compatibility. 
                        //TODO: do we really need this?
                        _class = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_class));
                    }
                    catch (Exception)
                    {

                        throw (new TenorException("Invalid \'cl\' parameter. Value: " + _class));
                    }
                }


                Type classeT = null;

                Assembly ass;

                lock (messagesLock)
                {
                    try
                    {
                        ass = Array.Find<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), new Predicate<Assembly>(FindAssembly));
                        if (ass != null)
                        {
                            classeT = ass.GetType(_class, false, true);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (classeT == null)
                {
                    //We cannot find the type, try on other assemblies.
                    foreach (string file in System.IO.Directory.GetFiles(AppDomain.CurrentDomain.RelativeSearchPath, "*.dll"))
                    {
                        try
                        {
                            Assembly ass2 = Assembly.LoadFile(file);
                            if (ass2 != null)
                            {
                                classeT = ass2.GetType(_class, false, true);
                                if (classeT != null)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (classeT == null)
                    {
                        throw (new TenorException("Cannot find class \'" + _class + "\'."));
                        //return; //404
                    }
                    //Else
                    //    classeT = ass.GetType(_class, False, True)
                }

                //Search for a constructor that have an integer parameter.
                //TODO: Make this mor flexible, with more than one parameter and type.
                ConstructorInfo classeC = classeT.GetConstructor(new Type[] { typeof(int) });

                if (classeC == null)
                {
                    //no constructor found.
                    throw (new TenorException("Cannot find any suitable constructor to call for class \'" + _class + "\'."));
                    //return; //404
                }
                //parse parameters.
                string valorO = QueryString("p1");
                int valorI = 0;
                if (!int.TryParse(valorO, out valorI))
                {
                    //invalid parameter
                    throw (new TenorException("Invalid parameter value."));
                    //return; //404
                }
                else
                {
                    try
                    {
                        object instance = classeC.Invoke(new object[] { valorI });
                        IResponseObject responseObject = null;

                        if (classeT.GetInterface(typeof(IResponseObject).FullName) == null)
                        {
                            PropertyInfo prop = Array.Find<PropertyInfo>(classeT.GetProperties(), new Predicate<PropertyInfo>(FindProperty));
                            if (prop != null)
                            {
                                responseObject = (IResponseObject)(prop.GetValue(instance, null));
                            }
                        }
                        else
                        {
                            responseObject = (IResponseObject)instance;
                        }
                        if (responseObject == null)
                        {
                            //This class does not have IResponseObject nor a property that returns one. 
                            throw (new TenorException("Invalid response stream."));
                            //return; //404
                        }

                        IImage img = responseObject as IImage;
                        if (img != null)
                        {
                            if (!string.IsNullOrEmpty(QueryString("w")) || !string.IsNullOrEmpty(QueryString("h")) || !string.IsNullOrEmpty(QueryString("l")))
                            {
                                //image manipulation.
                                int l = 0;
                                int.TryParse(QueryString("l"), out l);



                                int w = 0;
                                int.TryParse(QueryString("w"), out w);
                                int h = 0;
                                int.TryParse(QueryString("h"), out h);

                                ResizeMode mode = ResizeMode.Proportional;

                                if (!string.IsNullOrEmpty(QueryString("m")))
                                {
                                    try
                                    {
                                        mode = (ResizeMode)(int.Parse(QueryString("m")));
                                    }
                                    catch (Exception)
                                    {
                                        throw (new HttpException("Invalid image parameters", 500));
                                    }
                                }

                                if (w > 0 || h > 0)
                                {
                                    img.Resize(w, h, mode);
                                }
                                if (l != 0)
                                {
                                    img.LowQuality = true;
                                }
                                //If crop Then
                                //    img.Resize(w, h, Drawing.ResizeMode.Crop)
                                //Else
                                //    If w = 0 Then
                                //        img.ResizeByHeight(h)
                                //    ElseIf h = 0 Then
                                //        img.ResizeByWidth(w)
                                //    Else
                                //        img.Resize(w, h, Drawing.ResizeMode.Stretch)
                                //    End If
                                //End If
                                img = null;
                            }
                        }


                        Stream File = null;
                        File = responseObject.WriteContent();

                        CacheData cacheData = new CacheData();
                        cacheData.Expires = Tenor.Configuration.TenorModule.DefaultExpiresTime;
                        cacheData.ContentType = responseObject.ContentType;
                        cacheData.ContentLength = File.Length;


                        if (!string.IsNullOrEmpty(QueryString("fn")))
                        {
                            cacheData.FileName = QueryString("fn");
                            if (!cacheData.FileName.Contains("."))
                            {
                                cacheData.FileName += "." + IO.BinaryFile.GetExtension(cacheData.ContentType);
                            }
                        }
                        else
                        {
                            cacheData.FileName = valorI.ToString() + "." + IO.BinaryFile.GetExtension(cacheData.ContentType);
                        }


                        if (!string.IsNullOrEmpty(QueryString("dl")))
                        {
                            try
                            {
                                cacheData.ForceDownload = Convert.ToBoolean(int.Parse(QueryString("dl")));
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (string.IsNullOrEmpty(cacheData.ContentType))
                        {
                            cacheData.ContentType = GetMimeType(File);
                        }

                        WriteHeaders(app, cacheData);
                        WriteStream(File, app);


                        cacheData.Object = File;
                        if (app.Request[Tenor.Configuration.TenorModule.NoCache] == string.Empty)
                        {
                            lock (messagesLock)
                            {
                                object obj;
                                obj = Cache.Add("instance:" + app.Request.QueryString.ToString(), cacheData, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Default, new System.Web.Caching.CacheItemRemovedCallback(Cache_onItemRemoved));
                            }
                        }

                    }
                    catch (Exception)
                    {
                        //app.Context.ClearError()
                        //app.Context.AddError(New HttpException(500, "server error", ex.InnerException))
                        //app.Context.Response.StatusCode = 500
                        throw;
                    }
                }
            }
        }
    }
}