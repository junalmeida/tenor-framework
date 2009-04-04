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
        private const string propertyFieldEnd = "$endpropertyfield$";
        private const string propertiesTempMark = "$properties$";

        private const string lazyPropertyField = "$lazypropertyfield$";
        private const string lazyPropertyFieldEnd = "$endlazypropertyfield$";

        private const string liststart = "$list$";
        private const string listend = "$endlist$";
        private const string listsep = "$$"; 
        private const string listTempMark = "$list$";

        private const string constFields = "$constfields$";
        private const string constFieldsEnd = "$endconstfields$";
        private const string constTempMark = "$constants$";

        private const string fkHeader = "$fkheader$";
        private const string fkHeaderEnd = "$endfkheader$";

        private const string propertyToMany = "$propertytomany$";
        private const string propertyToManyEnd = "$endpropertytomany$";
        private const string propertiesToManyTempMark = "$propertiestomany$";

        private const string propertyToOne = "$propertytoone$";
        private const string propertyToOneEnd = "$endpropertytoone$";
        private const string propertiesToOneTempMark = "$propertiestoone$";

        private const string paramList = "$paramlist$";
        private const string paramListEnd = "$endparamlist$";
        private const string ctorParamsTempMark = "$ctorparams$";

        private const string ctorBody = "$ctorbody$";
        private const string ctorBodyEnd = "$endctorbody$";
        private const string ctorBodyTempMark = "$ctorbodymark$";

        private string propertyTemplate;
        private string lazyPropertyTemplate;
        private Dictionary<HeaderItem, string> propertyHeaderList = new Dictionary<HeaderItem, string>();
        private string constantTemplate;
        private string propertyToManyTemplate;
        private string propertyToManyTemplateHeader;

        private string propertyToOneTemplate;
        private string propertyToOneTemplateHeader;
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
                int pos2 = template.IndexOf(propertyFieldEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyTemplate = template.Substring(pos + propertyField.Length, pos2 - (pos + propertyField.Length));
                template = template.Remove(pos, pos2 - pos + propertyFieldEnd.Length);
                template = template.Insert(pos, propertiesTempMark);
                //end properties mark
                //lazy properties
                pos = template.IndexOf(lazyPropertyField);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(lazyPropertyFieldEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                lazyPropertyTemplate = template.Substring(pos + lazyPropertyField.Length, pos2 - (pos + lazyPropertyField.Length));
                template = template.Remove(pos, pos2 - pos + lazyPropertyFieldEnd.Length);
                //end lazy properties mark
                //prop header list
                pos = propertyTemplate.IndexOf(liststart);
                if (pos != -1)
                {
                    pos2 = propertyTemplate.IndexOf(listend, pos);
                    string hParamList = propertyTemplate.Substring(pos + liststart.Length, pos2 - (pos + liststart.Length));
                    propertyTemplate = propertyTemplate.Remove(pos, pos2 - pos + listend.Length);
                    propertyTemplate = propertyTemplate.Insert(pos, listTempMark);
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
                pos2 = template.IndexOf(constFieldsEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                constantTemplate = template.Substring(pos + constFields.Length, pos2 - (pos + constFields.Length));
                template = template.Remove(pos, pos2 - pos + constFieldsEnd.Length);
                template = template.Insert(pos, constTempMark);
                //end constants mark
                //property toMany
                pos = template.IndexOf(propertyToMany);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(propertyToManyEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyToManyTemplate = template.Substring(pos + propertyToMany.Length, pos2 - (pos + propertyToMany.Length));
                template = template.Remove(pos, pos2 - pos + propertyToManyEnd.Length);
                template = template.Insert(pos, propertiesToManyTempMark);


                pos = propertyToManyTemplate.IndexOf(fkHeader);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = propertyToManyTemplate.IndexOf(fkHeaderEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyToManyTemplateHeader = propertyToManyTemplate.Substring(pos + fkHeader.Length, pos2 - (pos + fkHeader.Length));
                propertyToManyTemplate = propertyToManyTemplate.Remove(pos, pos2 - pos + fkHeaderEnd.Length);
                propertyToManyTemplate = propertyToManyTemplate.Insert(pos, listTempMark);

                //end property
                //property toOne
                pos = template.IndexOf(propertyToOne);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(propertyToOneEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyToOneTemplate = template.Substring(pos + propertyToOne.Length, pos2 - (pos + propertyToOne.Length));
                template = template.Remove(pos, pos2 - pos + propertyToOneEnd.Length);
                template = template.Insert(pos, propertiesToOneTempMark);


                pos = propertyToOneTemplate.IndexOf(fkHeader);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = propertyToOneTemplate.IndexOf(fkHeaderEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                propertyToOneTemplateHeader = propertyToOneTemplate.Substring(pos + fkHeader.Length, pos2 - (pos + fkHeader.Length));
                propertyToOneTemplate = propertyToOneTemplate.Remove(pos, pos2 - pos + fkHeaderEnd.Length);
                propertyToOneTemplate = propertyToOneTemplate.Insert(pos, listTempMark);

                //end property
                //constructor params
                pos = template.IndexOf(paramList);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(paramListEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                paramItem = template.Substring(pos + paramList.Length, pos2 - (pos + paramList.Length));
                template = template.Remove(pos, pos2 - pos + paramListEnd.Length);
                template = template.Insert(pos, ctorParamsTempMark);
                //end constuctor params
                //constructor body
                pos = template.IndexOf(ctorBody);
                if (pos == -1)
                    throw new TemplateException();
                pos2 = template.IndexOf(ctorBodyEnd, pos);
                if (pos2 == -1)
                    throw new TemplateException();
                ctorItem = template.Substring(pos + ctorBody.Length, pos2 - (pos + ctorBody.Length));
                template = template.Remove(pos, pos2 - pos + ctorBodyEnd.Length);
                template = template.Insert(pos, ctorBodyTempMark);
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
            IForeignKeys foreigns = null;
            string fileName = null;
            string className = null;
            if (table != null)
            {
                colunas = table.Columns;
                foreigns = table.ForeignKeys;
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
            GenerateForeigns(className, foreigns);
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
            List<string> ctorparams = new List<string>();
            List<string> ctorbodymark = new List<string>();
            List<string> constants = new List<string>();
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
                constants.Add(this.constantTemplate.Replace("$propertyname$", propertyName));

                if (column.IsInPrimaryKey)
                {
                    string ctoritem = this.paramItem;
                    ctoritem = ctoritem.Replace("$propertyname$", propertyName);
                    ctoritem = ctoritem.Replace("$elementtype$", column.LanguageType);
                    ctorparams.Add(ctoritem);

                    ctoritem = this.ctorItem;
                    ctoritem = ctoritem.Replace("$propertyname$", propertyName);
                    ctorbodymark.Add(ctoritem);
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

                item = item.Replace(Template.listTempMark, string.Join(", ", headers.ToArray()));

                list.Add(item);

            }


            template = template.Replace(Template.propertiesTempMark, string.Join(Environment.NewLine, list.ToArray()));
            template = template.Replace(Template.constTempMark, string.Join(string.Empty, constants.ToArray()));
            template = template.Replace(Template.ctorParamsTempMark, string.Join(", ", ctorparams.ToArray()));
            template = template.Replace(Template.ctorBodyTempMark, string.Join(string.Empty, ctorbodymark.ToArray()));
        }

        private void GenerateForeigns(string className, IForeignKeys foreigns)
        {
            List<string> foreignKeysToOne = new List<string>();
            List<string> foreignKeysToMany = new List<string>();

            if (foreigns != null)
                foreach (IForeignKey fk in foreigns)
                {

                    if (fk.ForeignTable.Equals(table))
                    {
                        string item = this.propertyToOneTemplate;

                        string name = fk.PrimaryTable.Name;
                        string schema = fk.PrimaryTable.Schema;
                        if (schema != string.Empty && schema != "dbo" && schema != table.Schema)
                            schema = schema + ".";
                        else
                            schema = string.Empty;

                        item = item.Replace("$elementtype$", schema + name);
                        item = item.Replace("$propertyname$", name);
                        item = item.Replace("$fkname$", fk.Name);

                        List<string> headers = new List<string>();
                        for (int i = 0; i < fk.ForeignColumns.Count; i++)
                        {
                            string header = this.propertyToOneTemplateHeader;
                            header = header.Replace("$elementtype$", this.baseNamespace + "." + schema + name);
                            header = header.Replace("$foreign$", fk.PrimaryColumns[i].Name);
                            header = header.Replace("$local$", fk.ForeignColumns[i].Name);
                            headers.Add(header);
                        }
                        item = item.Replace(Template.listTempMark, string.Join(", ", headers.ToArray()));
                        foreignKeysToOne.Add(item);
                    }
                    else if (fk.PrimaryTable.Equals(table) && fk.ForeignTable.PrimaryKeys.Count == 1 && fk.PrimaryColumns.Count == 1 && fk.ForeignColumns[0].IsInPrimaryKey)
                    {
                        string item = this.propertyToOneTemplate;

                        string name = fk.ForeignTable.Name;
                        string schema = fk.ForeignTable.Schema;
                        if (schema != string.Empty && schema != "dbo" && schema != table.Schema)
                            schema = schema + ".";
                        else
                            schema = string.Empty;

                        item = item.Replace("$elementtype$", schema + name);
                        item = item.Replace("$propertyname$", name);
                        item = item.Replace("$fkname$", fk.Name);



                        List<string> headers = new List<string>();
                        for (int i = 0; i < fk.ForeignColumns.Count; i++)
                        {
                            string header = this.propertyToOneTemplateHeader;
                            header = header.Replace("$elementtype$", this.baseNamespace + "." + schema + name);
                            header = header.Replace("$foreign$", fk.ForeignColumns[i].Name);
                            header = header.Replace("$local$", fk.PrimaryColumns[i].Name);
                            headers.Add(header);
                        }
                        item = item.Replace(Template.listTempMark, string.Join(", ", headers.ToArray()));
                        foreignKeysToOne.Add(item);
                    }
                    else if (fk.PrimaryTable.Equals(table))
                    {
                        string item = this.propertyToManyTemplate;

                        string name = fk.ForeignTable.Name;
                        string schema = fk.ForeignTable.Schema;
                        if (schema != string.Empty && schema != "dbo" && schema != table.Schema)
                            schema = schema + ".";
                        else
                            schema = string.Empty;

                        item = item.Replace("$elementtype$", schema + name);
                        item = item.Replace("$propertyname$", name);
                        item = item.Replace("$fkname$", fk.Name);



                        List<string> headers = new List<string>();
                        for (int i = 0; i < fk.ForeignColumns.Count; i++)
                        {
                            string header = this.propertyToManyTemplateHeader;
                            header = header.Replace("$elementtype$", this.baseNamespace + "." + schema + name);
                            header = header.Replace("$foreign$", fk.ForeignColumns[i].Name);
                            header = header.Replace("$local$", fk.PrimaryColumns[i].Name);
                            headers.Add(header);
                        }
                        item = item.Replace(Template.listTempMark, string.Join(", ", headers.ToArray()));
                        foreignKeysToMany.Add(item);
                    }
                }
            template = template.Replace(Template.propertiesToManyTempMark, string.Join(Environment.NewLine, foreignKeysToMany.ToArray()));
            template = template.Replace(Template.propertiesToOneTempMark, string.Join(Environment.NewLine, foreignKeysToOne.ToArray()));

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
