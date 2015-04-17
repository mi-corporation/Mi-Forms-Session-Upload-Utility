// GetData - standard methods to retrieve data from a source
// Kevin Burgess - 04/07/2015
// Mi-Corporation

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiCo.MiForms.Uploader
{
    public static class GetData
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Query a database and return a set of values
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="filterValue">Value of the item to use in a WHERE clause</param>
        /// <returns>Dataset of values from a View</returns>
        public static DataTable QueryDatabase(String table, String column, String where)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM [" + Config.Table + "] WHERE [" + Config.Column + "] = '" + Config.Where + "'", conn);
                    var dr = cmd.ExecuteReader();
                    dt.Load(dr);
                    conn.Close();
                }

                _log.Info("Collected data from database...");
            }
            catch (Exception ex)
            {
                // The connection failed. Display an error message.
                _log.Error(ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// Query a stored procedure using a list of parameters
        /// This function assumes you have a stored procedure already in your database.
        /// Also note that it only works if for Text. If your procedure expects another data type it will fail.
        /// </summary>
        /// <returns>DataTable of values</returns>
        public static DataTable QueryStoredProcedure(String name, String[] parameters, String[] values)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    for (int i = 0; i < parameters.Length; i++)
			        {
			            cmd.Parameters.Add(parameters[i], SqlDbType.VarChar).Value = values[i];
			        }
                    var dr = cmd.ExecuteReader();
                    dt.Load(dr);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                // The connection failed. Display an error message.
                _log.Error(ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// Collects all of the files that match the parameters in the config file.
        /// </summary>
        /// <returns>A list of files with paths</returns>
        public static IEnumerable<String> CollectFiles()
        {
            // look for files in the FilePath specified in App.config
            var files = Directory.GetFiles(Config.FilePath).ToList();
            IEnumerable<String> collectedFiles = null;

            if (files.Count() > 0)
            {
                // check if the files are of the FileType specified in App.config
                collectedFiles = from f in files
                                 where Path.GetExtension(f).ToLower() == Config.FileType.ToLower()
                                 select f;
            }

            // if no files exist exit
            if (collectedFiles.Count() < 1)
            {
                _log.Info("There are no files to process.");
            }
            return collectedFiles;
        }
    }
}
