using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Migrations;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace EzCMS.Entity.Core.SiteInitialize
{
    public class SiteInitializer
    {
        private const string SiteConfigFileRelativePath = "/App_Data/Site.config";
        private const string ApplicationKey = "EzSiteConfiguration";

        protected static string SiteConfigFile
        {
            get { return HostingEnvironment.MapPath(SiteConfigFileRelativePath); }
        }

        public static bool SiteConfigError
        {
            get
            {
                if (HttpContext.Current.Application["SiteConfigError"] != null)
                {
                    return (bool)HttpContext.Current.Application["SiteConfigError"];
                }

                return false;
            }
            set
            {
                HttpContext.Current.Application["SiteConfigError"] = value;
            }
        }

        #region Get site configuration

        /// <summary>
        /// Get the site configuration
        /// </summary>
        /// <returns></returns>
        public static SiteConfiguration GetHttpSiteConfiguration()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return null;
            }

            // Try to load from application
            var configuration = httpContext.Application[ApplicationKey] as SiteConfiguration;
            if (configuration == null)
            {
                configuration = GetConfiguration();

                httpContext.Application[ApplicationKey] = configuration;
            }
            return configuration;
        }

        /// <summary>
        /// Check if site is fully setup or not
        /// </summary>
        /// <returns></returns>
        public static bool IsSetupFinish()
        {
            var siteConfig = GetConfiguration();

            if (siteConfig != null && siteConfig.IsSetupFinish)
                return true;

            return false;
        }

        /// <summary>
        /// Get configuration file and parse
        /// </summary>
        /// <returns></returns>
        public static SiteConfiguration GetConfiguration()
        {
            if (File.Exists(SiteConfigFile))
            {
                var serializer = new XmlSerializer(typeof(SiteConfiguration), new[] { typeof(Plugin) });

                using (var configFile = File.OpenRead(SiteConfigFile))
                {
                    return (SiteConfiguration)serializer.Deserialize(configFile);
                }
            }
            return null;
        }

        /// <summary>
        /// Reset the configuration
        /// </summary>
        /// <returns></returns>
        public static bool ResetConfiguration(bool restartAppConfig = true)
        {
            //Remove site config file
            if (File.Exists(SiteConfigFile))
            {
                File.Delete(SiteConfigFile);

                var httpContext = HttpContext.Current;
                if (httpContext != null && restartAppConfig)
                {
                    httpContext.Application[ApplicationKey] = null;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="restartAppConfig"></param>
        public static void SaveConfiguration(SiteConfiguration config, bool restartAppConfig = true)
        {
            ResetConfiguration(restartAppConfig);

            var serializer = new XmlSerializer(typeof(SiteConfiguration), new[] { typeof(Plugin) });
            string directory = Path.GetDirectoryName(SiteConfigFile);
            if (directory != null) Directory.CreateDirectory(directory);
            using (var configFile = File.OpenWrite(SiteConfigFile))
            {
                serializer.Serialize(configFile, config);
            }
        }

        #endregion

        #region Setup database

        /// <summary>
        /// Test the connection string and permissions
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="integratedSecurity"></param>
        /// <returns></returns>
        public ResponseModel TestConnectionString(string server, string database, string userName, string password, bool integratedSecurity)
        {
            SqlCommand command;
            #region Check if user can login into the server

            var sqlConnectionString = string.Format(
                 "Data Source={0};uid={1};pwd={2};Integrated Security={3};MultipleActiveResultSets=True",
                 server, userName, password,
                 integratedSecurity ? "True" : "False");

            var conn = new SqlConnection(sqlConnectionString);
            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            #endregion

            #region Check if current database is clean or not

            var fullConnectionString = string.Format(
                 "Data Source={0};Initial Catalog={1};uid={2};pwd={3};Integrated Security={4};MultipleActiveResultSets=True",
                 server, database, userName, password,
                 integratedSecurity ? "True" : "False");
            var fullConn = new SqlConnection(fullConnectionString);
            try
            {
                //Check if table exists
                fullConn.Open();

                const string query = "select count(*) from information_schema.tables";
                command = new SqlCommand(query, fullConn);

                var tableCount = (int)command.ExecuteScalar();
                if (tableCount > 0)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message =
                            "Database is not clean. Please choose other database or remove all tables in it."
                    };
                }

                //database is not existed in system, then we check if they can create database or not
                const string checkCreateDatabasePermissionQuery = "SELECT HAS_PERMS_BY_NAME(@database, 'DATABASE', 'CREATE TABLE')";
                command = new SqlCommand(checkCreateDatabasePermissionQuery, conn);
                command.Parameters.AddWithValue("@database", database);
                var canCreateTable = (int)command.ExecuteScalar() == 1;

                if (!canCreateTable)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message =
                            "User do not have permission to create table. Please give the user permission to create table in selected database."
                    };
                }
            }
            catch (Exception)
            {
                //database is not existed in system, then we check if they can create database or not
                const string checkCreateDatabasePermissionQuery = "SELECT HAS_PERMS_BY_NAME(null, null, 'CREATE ANY DATABASE');";
                command = new SqlCommand(checkCreateDatabasePermissionQuery, conn);
                var canCreateDatabase = (int)command.ExecuteScalar() == 1;

                if (!canCreateDatabase)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message =
                            "The database is not existed or user do not have permission to create database. Please create database first and assign user as db owner or give the user permission to create database."
                    };
                }
            }
            finally
            {
                CloseConnection(conn);
                CloseConnection(fullConn);
            }

            #endregion


            return new ResponseModel
            {
                Success = true,
                Message = "Your setting is saving successfully."
            };
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <param name="connection"></param>
        public static void CloseConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Init database
        /// </summary>
        public void InitDatabase()
        {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EzCMSEntities, Configuration>());

            using (var dbContext = new EzCMSEntities())
            {
                //Trigger database to create structure and data
                var trigger = dbContext.Users.ToList();
            }
        }

        #endregion
    }
}