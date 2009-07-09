using System;
using System.Configuration;

namespace Tenor.Configuration
{
    /// <summary>
    /// Encapsulates all Tenor Framework specific settings defined on app.config or web.config. 
    /// </summary>
    public sealed class Tenor : ConfigurationSection
    {
        private Tenor() { }

        private static Tenor current;
        /// <summary>
        /// Gets the Tenor.Configuration.Tenor object the represents the current context.
        /// </summary>
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

        /// <summary>
        /// Gets the an array of user-defined database dialects. 
        /// </summary>
        [ConfigurationProperty("dialects")]
        public DialectsSection Dialects
        {
            get
            {
                return (DialectsSection)this[this.Properties["dialects"]];
            }
        }

        /// <summary>
        /// Gets an object that defines settings for exception handling.
        /// </summary>
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
