using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiCo.MiForms.Uploader
{
    public static class Config
    {
        // server settings
        public static string Server = ConfigurationManager.AppSettings["Server"];

        public static int Port 
        { 
            get 
            {
                int value = 25;
                Int32.TryParse(ConfigurationManager.AppSettings["Port"], out value);
                return value;
            }
        }
        public static string Prefix = ConfigurationManager.AppSettings["URLPrefix"];
        public static string Customer = ConfigurationManager.AppSettings["Customer"];
        public static string Username = ConfigurationManager.AppSettings["Username"];
        public static string Password = ConfigurationManager.AppSettings["Password"];

        // form template settings
        public static string FormId = ConfigurationManager.AppSettings["FormId"];

        // file settings
        public static Boolean IsFileCollectionUsed
        {
            get
            {
                Boolean value;
                Boolean.TryParse(ConfigurationManager.AppSettings["IsFileCollectionUsed"], out value);
                return value;
            }
        }
        public static string FilePath = ConfigurationManager.AppSettings["FilePath"];
        public static string FileType = ConfigurationManager.AppSettings["FileType"];

        // sql settings
        public static Boolean IsDbCollectionUsed
        {
            get
            {
                Boolean value;
                Boolean.TryParse(ConfigurationManager.AppSettings["IsDbCollectionUsed"], out value);
                return value;
            }
        }
        public static string Table = ConfigurationManager.AppSettings["DbTable"];
        public static string Column = ConfigurationManager.AppSettings["DbColumn"];
        public static string Where = ConfigurationManager.AppSettings["DbWhere"];
        public static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

        public static Boolean IsStoredProcedureCollectionUsed
        {
            get
            {
                Boolean value;
                Boolean.TryParse(ConfigurationManager.AppSettings["IsStoredProcedureCollectionUsed"], out value);
                return value;
            }
        }
        public static string StoredProcedureName = ConfigurationManager.AppSettings["StoredProcedureName"];
        public static String[] StoredProcedureParameters
        {
            get
            {
                var parameters = ConfigurationManager.AppSettings["StoredProcedureParameters"];
                return parameters.Split(',');
            }
        }
        public static String[] StoredProcedureValues
        {
            get
            {
                var values = ConfigurationManager.AppSettings["StoredProcedureValues"];
                return values.Split(',');
            }
        }

        // email settings
        public static string SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        public static string SmtpPort = ConfigurationManager.AppSettings["SmtpPort"];
        public static string EmailFrom = ConfigurationManager.AppSettings["SmtpEmail"];


    }
}
