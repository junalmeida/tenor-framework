using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Tenor.Configuration
{
    /// <summary>
    /// Represents the dialect item of a configuration file.
    /// </summary>
    public class DialectElement : ConfigurationElement
    {

        /// <summary>
        /// Gets the provider name supplied.
        /// </summary>
        [ConfigurationProperty("providerName", IsKey = true)]
        public string ProviderName
        {
            get { return (string)this[this.Properties["providerName"]]; }
        }


        /// <summary>
        /// Gets a string with a fullname of a type that implements the current dialect.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this[this.Properties["type"]]; }
        }
    }

    /// <summary>
    /// Represents the dialects section of a configuration file.
    /// </summary>
    public class DialectsSection : ConfigurationElement, System.Collections.IEnumerable
    {
        /// <summary>
        /// Gets the list of defined dialects.
        /// </summary>
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public DialectsCollection Dialects
        {
            get
            {
                return (DialectsCollection)this[this.Properties[""]];
            }
        }

        #region IEnumerable Members

        /// <summary>
        /// Gets an System.Collection.IEnumerator which is used to iterate through the DialectsCollection.
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return Dialects.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Represents a list of configured dialects.
    /// </summary>
    [ConfigurationCollection(typeof(DialectElement))]
    public class DialectsCollection : ConfigurationElementCollection
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
