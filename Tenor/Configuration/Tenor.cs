using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Tenor.Configuration
{
    public sealed class Tenor : ConfigurationSection
    {
        private Tenor() : base() { }

        private static Tenor current;
        public static Tenor Current
        {
            get
            {
                if (current == null)
                {
                    try
                    {
                        current = (Tenor)ConfigurationManager.GetSection(typeof(Tenor).Name.ToLower());
                    }
                    catch (Exception ex)
                    {
                        throw new System.Configuration.ConfigurationErrorsException("Cannot load Tenor configuration. Please, see documentation on configuration files.", ex);
                    }
                }
                if (current == null)
                    current = new Tenor();

                return current;
            }
        }


        [ConfigurationProperty("dialects")]
        public DialectsSection Dialects
        {
            get
            {
                return (DialectsSection)this[this.Properties["dialects"]];
            }
        }


        [ConfigurationProperty("exceptions")]
        public ExceptionsSection Exceptions
        {
            get
            {
                return (ExceptionsSection)this[this.Properties["exceptions"]];
            }
        }

    }
}
