using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Tenor.Web.UI.WebControls.Design
{

    /// <summary>
    /// Manage flash version strings. This code is used by the designer.
    /// </summary>
    public class FlashVersionConverter : System.ComponentModel.StringConverter
    {


        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            List<string> values = new List<string>();
            values.Add("6.0.0.0");
            values.Add("7.0.0.0");
            values.Add("8.0.0.0");
            values.Add("9.0.28.0");

            System.ComponentModel.TypeConverter.StandardValuesCollection res = new System.ComponentModel.TypeConverter.StandardValuesCollection(values);
            return res;
        }


    }
}