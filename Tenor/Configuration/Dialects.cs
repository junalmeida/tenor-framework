using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Tenor.Configuration
{
    public class DialectElement : ConfigurationElement
    {


        [ConfigurationProperty("providerName", IsKey = true)]
        public string ProviderName
        {
            get { return (string)this[this.Properties["providerName"]]; }
        }



        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this[this.Properties["type"]]; }
        }
    }

    public class DialectsSection : ConfigurationElement, System.Collections.IEnumerable
    {
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public DialecsCollection Dialects
        {
            get
            {
                return (DialecsCollection)this[this.Properties[""]];
            }
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return Dialects.GetEnumerator();
        }

        #endregion
    }

    [ConfigurationCollection(typeof(DialectElement))]
    public class DialecsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DialectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DialectElement)element).ProviderName;

        }

    }


}
