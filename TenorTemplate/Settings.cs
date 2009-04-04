using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
namespace TenorTemplate
{
    [Serializable(), XmlRoot("TenorTemplate")]
    public class Settings
    {
        private Settings()
        {
        }


        private MyMeta.dbDriver? driver;

        public MyMeta.dbDriver? Driver
        {
            get { return driver; }
            set { driver = value; }
        }

        private string catalog;

        public string Catalog
        {
            get { return catalog; }
            set { catalog = value; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }


        private string server;

        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        private string directory;

        public string Directory
        {
            get { return directory; }
            set { directory = value; }
        }
        private string baseNamespace;

        public string BaseNamespace
        {
            get { return baseNamespace; }
            set { baseNamespace = value; }
        }
        private Language language;

        public Language Language
        {
            get { return language; }
            set { language = value; }
        }

        private static string FileName
        {
            get
            {
                return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar +  "settings.xml";
            }
        }

        public static Settings Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            FileStream file = null;
            try
            {
                file = new FileStream(FileName, FileMode.Open);
                Settings config = (Settings)serializer.Deserialize(file);

                return config;
            }
            catch (Exception)
            {
                return new Settings();
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            FileStream file = null;
            try
            {
                file = new FileStream(FileName, FileMode.OpenOrCreate);
                serializer.Serialize(file, this);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
    }
}
