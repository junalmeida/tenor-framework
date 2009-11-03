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
    /// <summary>
    /// Represents the email item of a configuration file.
    /// </summary>
    public class EmailElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the email.
        /// </summary>
        [ConfigurationProperty("email", IsKey = true)]
        public string Email
        {
            get { return (string)this[this.Properties["email"]]; }
        }

        /// <summary>
        /// Gets the display name used in email destinations.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string)this[this.Properties["name"]]; }
        }
    }

    public enum LogMode
    {
        None,
        Email,
        File
    }

    /// <summary>
    /// Represents the Exception section of a configuration file.
    /// </summary>
    public class ExceptionsSection : ConfigurationElement, System.Collections.IEnumerable
    {
        [ConfigurationProperty("logMode", DefaultValue = LogMode.None)]
        public LogMode LogMode
        {
            get
            {
                return (LogMode)this[this.Properties["logMode"]];
            }
        }

        [ConfigurationProperty("filePath", DefaultValue = "")]
        public string FilePath
        {
            get
            {
                return (string)this[this.Properties["filePath"]];
            }
        }

        /// <summary>
        /// Gets the list of defined emails.
        /// </summary>
        [ConfigurationProperty("emails", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public EmailsCollection Emails
        {
            get
            {
                return (EmailsCollection)this[this.Properties["emails"]];
            }
        }

        #region IEnumerable Members
        /// <summary>
        /// Gets an System.Collection.IEnumerator which is used to iterate through the EmailsCollection.
        /// </summary>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return Emails.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of emails.
    /// </summary>
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