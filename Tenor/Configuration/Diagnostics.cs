using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace Tenor.Configuration
{

    public class EmailElement : ConfigurationElement
    {
        [ConfigurationProperty("email", IsKey = true)]
        public string Email
        {
            get { return (string)this[this.Properties["email"]]; }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string)this[this.Properties["name"]]; }
        }
    }

    public class ExceptionsSection : ConfigurationElement, System.Collections.IEnumerable
    {
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public EmailsCollection Emails
        {
            get
            {
                return (EmailsCollection)this[this.Properties[""]];
            }
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return Emails.GetEnumerator();
        }

        #endregion
    }

    [ConfigurationCollection(typeof(DialectElement))]
    public class EmailsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EmailElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EmailElement)element).Email;

        }

    }
}