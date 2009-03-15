using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;
using MyMeta;


namespace TenorTemplate
{
    public enum Language
    {
        CSharp,
        VBNet
    }    

    public class Template
    {


        Language language;
        string template;
        string baseNamespace;

        private Template(Language language, string baseNamespace)
        {
            this.language = language;
            this.baseNamespace = baseNamespace;
            template = LoadTemplate();
        }

        ITable table;
        IView view;


        public Template(Language language, string baseNamespace, MyMeta.Single tableOrView)
            : this(language, baseNamespace)
        {
            this.table = tableOrView as ITable;
            if (this.table == null)
                this.view = tableOrView as IView;
        }

        private static object[] cachedTemplate;
        private string LoadTemplate()
        {
            try
            {

                string resource = string.Format("TenorTemplate.Resources.template_{0}.txt", language.ToString().ToLower());
                string template;
                if (cachedTemplate == null || !string.Equals(cachedTemplate[0].ToString(), resource))
                {
                    Stream stream = this.GetType().Assembly.GetManifestResourceStream(resource);
                    if (stream == null)
                        throw new InvalidOperationException();

                    StreamReader reader = new StreamReader(stream);
                    template = reader.ReadToEnd();
                    cachedTemplate = new object[] { resource, template };
                }
                else
                {
                    template = (string)cachedTemplate[1];
                }
                return template;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not load the template.", ex);
            }
        }

        private enum HeaderItem
        {
            FieldName,
            PrimaryKey,
            AutoNumber,
            LazyLoading
        }

        private const string propertyField = "$propertyfield$";
        private const string endPropertyField = "$endpropertyfield$";
        private const string properties = "$properties$";

        private const string lazyPropertyField = "$lazypropertyfield$";
        private const string endLazyPropertyField = "$endlazypropertyfield$";

        private const string liststart = "$list$";
        private const string listend = "$endlist$";
        private const string listsep = "$$"; 
        private const string list = "$list$";

        private const string constFields = "$constfields$";
        private const string endConstFields = "$endconstfields$";
        private const string constants = "$constants$";

        private const string propertyToMany = "$propertytomany$";
        private const string endPropertyToMany = "$endpropertytomany$";
        private const string propertiesToMany = "$propertiestomany$";

        private const string propertyToOne = "$propertytoone$";
        private const string endPropertyToOne = "$endpropertytoone$";
        private const string propertiesToOne = "$propertiestoone$";

        private const string paramList = "$paramlist$";
        private const string endParamList = "$endparamlist$";
        private const string paramsMark = "$ctorparams$";

        private const string ctorBody = "$ctorbody$";
        private const string endCtorBody = "$endctorbody$";
        private const string ctorBodyMark = "$ctorbodymark$";

        private string propertyTemplate;
        private string lazyPropertyTemplate;
        private Dictionary<HeaderItem, string> propertyHeaderList = new Dictionary<HeaderItem, string>();
        private string constantTemplate;
        private string propertyToManyTemplate;
        private string propertyToOneTemplate;
        private string paramItem;
        private string ctorItem;
        private void ParseTemplate()
        {
            try
            {
                //properties
                int pos = template.IndexOf(propertyField);
                if (pos == -1)
                    throw new TemplateException();
                int pos2 = template.IndexOf(endPropertyField, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyTemplate = template.Substring(pos + propertyField.Length, pos2 - (pos + propertyField.Length));
                template = template.Remove(pos, pos2 - pos + endPropertyField.Length);
                template = template.Insert(pos, properties);
                //end properties mark
                //lazy properties
                pos = template.IndexOf(lazyPropertyField);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(endLazyPropertyField, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                lazyPropertyTemplate = template.Substring(pos + lazyPropertyField.Length, pos2 - (pos + lazyPropertyField.Length));
                template = template.Remove(pos, pos2 - pos + endLazyPropertyField.Length);
                //end lazy properties mark
                //prop header list
                pos = propertyTemplate.IndexOf(liststart);
                if (pos != -1)
                {
                    pos2 = propertyTemplate.IndexOf(listend, pos);
                    string hParamList = propertyTemplate.Substring(pos + liststart.Length, pos2 - (pos + liststart.Length));
                    propertyTemplate = propertyTemplate.Remove(pos, pos2 - pos + listend.Length);
                    propertyTemplate = propertyTemplate.Insert(pos, list);
                    foreach (string item in hParamList.Split(listsep.ToCharArray()))
                    {
                        foreach (HeaderItem header in Enum.GetValues(typeof(HeaderItem)))
                        {
                            if (item.Contains(header.ToString()))
                            {
                                propertyHeaderList.Add(header, item);
                                break;
                            }
                        }
                    }
                }
                //end prop header
                //constants
                pos = template.IndexOf(constFields);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(endConstFields, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                constantTemplate = template.Substring(pos + constFields.Length, pos2 - (pos + constFields.Length));
                template = template.Remove(pos, pos2 - pos + endConstFields.Length);
                template = template.Insert(pos, constants);
                //end constants mark
                //property toMany
                pos = template.IndexOf(propertyToMany);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(endPropertyToMany, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyToManyTemplate = template.Substring(pos + propertyToMany.Length, pos2 - (pos + propertyToMany.Length));
                template = template.Remove(pos, pos2 - pos + endPropertyToMany.Length);
                template = template.Insert(pos, propertiesToMany);
                //end property
                //property toOne
                pos = template.IndexOf(propertyToOne);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(endPropertyToOne, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyToOneTemplate = template.Substring(pos + propertyToOne.Length, pos2 - (pos + propertyToOne.Length));
                template = template.Remove(pos, pos2 - pos + endPropertyToOne.Length);
                template = template.Insert(pos, propertiesToOne);
                //end property
                //constructor params
                pos = template.IndexOf(paramList);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(endParamList, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                paramItem = template.Substring(pos + paramList.Length, pos2 - (pos + paramList.Length));
                template = template.Remove(pos, pos2 - pos + endParamList.Length);
                template = template.Insert(pos, paramsMark);
                //end constuctor params
                //constructor body
                pos = template.IndexOf(ctorBody);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(endCtorBody, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                ctorItem = template.Substring(pos + paramList.Length, pos2 - (pos + ctorBody.Length));
                template = template.Remove(pos, pos2 - pos + endCtorBody.Length);
                template = template.Insert(pos, ctorBodyMark);
                //end constuctor body
            }
            catch (TemplateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TemplateException(ex);
            }
        }


        public void Generate(string path)
        {
            string folder = null;
            ParseTemplate();
            
            IColumns colunas = null;
            string fileName = null;
            string className = null;
            if (table != null)
            {
                colunas = table.Columns;
                className = GenerateClass(table.Schema, table.Name, baseNamespace, table.Description, out folder);
            }
            else if (view != null)
            {
                colunas = view.Columns;
                className = GenerateClass(view.Schema, view.Name, baseNamespace, view.Description, out folder);
            }
            else
            {
                throw new InvalidOperationException();
            }
            GenerateProperties(className, colunas);

            fileName = className;
            switch (language)
            {
                case Language.CSharp:
                    fileName += ".cs";
                    break;
                case Language.VBNet:
                    fileName += ".vb";
                    break;
                default:
                    break;
            }
            
            if (!string.IsNullOrEmpty(folder))
            {
                path += Path.DirectorySeparatorChar + folder;
            }
            path += Path.DirectorySeparatorChar + "AutoGenerated";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            fileName = path + Path.DirectorySeparatorChar + fileName;
            if (File.Exists(fileName))
                File.Delete(fileName);

            StreamWriter file = new StreamWriter(fileName);
            file.Write(template);
            file.Close();
        }

        private string GenerateClass(string schema, string tableName, string currentNamespace, string description, out string nSpace)
        {
            nSpace = null;
            string className = tableName.Replace(" ", string.Empty);
            if (!string.IsNullOrEmpty(schema) && schema != "dbo")
            {
                nSpace = schema.Replace(" ", string.Empty);
                currentNamespace += "." + nSpace;
            }

            template = template.Replace("$classname$", className);
            template = template.Replace("$namespace$", currentNamespace);
            template = template.Replace("$schema$", schema);
            template = template.Replace("$tablename$", tableName);
            template = template.Replace("$tabledescription$", description);

            return className;
        }

        private void GenerateProperties(string className, IColumns colunas)
        {
            List<string> list = new List<string>();
            foreach (IColumn column in colunas)
            {
                bool isLazy = false;
                isLazy = column.LanguageType.ToLower().Contains("byte[]") || column.LanguageType.ToLower().Contains("byte()");

                string item;
                if (!isLazy)
                    item = propertyTemplate;
                else
                    item = lazyPropertyTemplate;

                bool useFieldName = false;
                string propertyName = column.Name;
                if (string.Equals(propertyName, className))
                {
                    propertyName = "Field" + propertyName;
                    useFieldName = true;
                }

                item = item.Replace("$propertyname$", propertyName);
                item = item.Replace("$fielddescription$", column.Description);
                item = item.Replace("$fieldname$", column.Name);
                item = item.Replace("$elementtype$", column.LanguageType);

                List<string> headers = new List<string>();
                if (useFieldName)
                    headers.Add(propertyHeaderList[HeaderItem.FieldName].Replace("$fieldname$", column.Name));
                if (column.IsAutoKey)
                    headers.Add(propertyHeaderList[HeaderItem.AutoNumber]);
                if (column.IsInPrimaryKey)
                    headers.Add(propertyHeaderList[HeaderItem.PrimaryKey]);

                item = item.Replace(Template.list, string.Join(", ", list.ToArray()));

                list.Add(item);

            }

            template = template.Replace(Template.properties, string.Join("\r\n", list.ToArray()));
        }
    }

    [Serializable]
    public class TemplateException : Exception
    {
        private const string message = "Invalid template structure.";
        public TemplateException() : base(message) { }
        public TemplateException(Exception inner) : base(message, inner) { }

    }
}
