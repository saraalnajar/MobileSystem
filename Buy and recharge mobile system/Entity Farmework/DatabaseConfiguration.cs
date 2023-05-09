using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;

namespace EntityFarmework
{
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public DatabaseConfiguration GetAppConfiguration()
        {
            DatabaseName = GetDatabaseName();
            ServerName = GetServerName();
            UserId = GetUserId();
            Password = GetPassword();
            return GetConfiguration();
        }

        public DatabaseConfiguration GetWebConfiguration()
        {
            DatabaseName = WebConfigurationManager.AppSettings["databaseName"];
            ServerName = WebConfigurationManager.AppSettings["ServerName"];
            UserId = WebConfigurationManager.AppSettings["UserId"];
            Password = WebConfigurationManager.AppSettings["Password"];
            return GetConfiguration();
        }

        public DatabaseConfiguration GetConfiguration()
        {
            DatabaseConfiguration configuration = new DatabaseConfiguration
            {
                ConnectionString =
                   "data source={ServerName};initial catalog={DBName};persist security info=True;user id={UserId};password={Password};multipleactiveresultsets=True;application name=EntityFramework",
                DatabaseName = DatabaseName,
                ServerName = ServerName,
                UserId = UserId,
                Password = Password
            };
            configuration.ConnectionString =
                configuration.ConnectionString.Replace("{DBName}", configuration.DatabaseName);
            configuration.ConnectionString =
                configuration.ConnectionString.Replace("{ServerName}", configuration.ServerName);
            configuration.ConnectionString =
                configuration.ConnectionString.Replace("{UserId}", configuration.UserId);
            configuration.ConnectionString =
                configuration.ConnectionString.Replace("{Password}", configuration.Password);
            return configuration;
        }
        
        public static string GetDataSource()
        {
            string dataSourceType = ConfigurationManager.AppSettings["DataSourceType"];
            return dataSourceType;
        }

        public static string GetPassword()
        {
            string password = ConfigurationManager.AppSettings["Password"];
            return password;
        }

        public static string GetUserId()
        {
            string userId = ConfigurationManager.AppSettings["UserId"];
            return userId;
        }

        public static string GetServerName()
        {
            string serverName = ConfigurationManager.AppSettings["ServerName"];
            return serverName;
        }

        public static string GetDatabaseName()
        {
            string databaseName = ConfigurationManager.AppSettings["databaseName"];
            return databaseName;
        }
    }
}
