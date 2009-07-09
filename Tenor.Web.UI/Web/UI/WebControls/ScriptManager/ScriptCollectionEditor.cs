using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Windows.Forms.Design;

namespace Tenor.Web.UI.WebControls.Design
{

    public class ScriptCollectionEditor : System.ComponentModel.Design.CollectionEditor
    {


        public ScriptCollectionEditor(Type type)
            : base(type)
        {
            types = new System.Type[] { typeof(ScriptBlockRightClick), typeof(ScriptBlockSelection), typeof(ScriptMasks) };
        }
        private Type[] types;

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }


        protected override System.Type[] CreateNewItemTypes()
        {
            return types;
        }

        protected override object CreateInstance(System.Type itemType)
        {
            ScriptManager man = (ScriptManager)this.Context.Instance;
            foreach (object i in man.Scripts)
            {
                if (i.GetType() == itemType)
                {
                    throw (new InvalidOperationException("You can have only one instance of each Script Object"));

                }
            }
            return base.CreateInstance(itemType);
        }

    }

    public class ScriptTypeConverter : System.ComponentModel.ExpandableObjectConverter
    {


        public ScriptTypeConverter()
        {

        }

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor) || destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }

        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string obj = value.ToString();
                if (!obj.StartsWith("Script"))
                {
                    return base.ConvertFrom(context, culture, value);
                }
                return this.GetType().Assembly.GetType(Configuration.Resources.AssemblyWebUIWebControls + "." + obj).GetConstructor(new System.Type[] { }).Invoke(new object[] { });
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }

        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
            //							if (destinationType == typeof(string))
            //							{
            //								return value.GetType().Name;
            //								}
            //								else if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            //								{

            //									return new System.ComponentModel.Design.Serialization.InstanceDescriptor(value.GetType().GetConstructor(new Type[] {}), new object[] {}, true);
            //									}
            //									else
            //									{
            //										return base.ConvertTo(context, culture, value, destinationType);
            //										}
        }


    }
}