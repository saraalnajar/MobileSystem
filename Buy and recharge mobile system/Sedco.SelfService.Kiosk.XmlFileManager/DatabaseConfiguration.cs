using System.Configuration;

namespace EntityFarmework
{
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public DatabaseConfiguration GetConfiguration()
        {
            string databaseName = ConfigurationManager.AppSettings["databaseName"];
            string serverName = ConfigurationManager.AppSettings["ServerName"];
            string userId = ConfigurationManager.AppSettings["UserId"];
            string password = ConfigurationManager.AppSettings["Password"];

            var configuration = new DatabaseConfiguration
            {
                ConnectionString = "data source={ServerName};initial catalog={DBName};persist security info=True;user id={UserId};password={Password};multipleactiveresultsets=True;application name=EntityFramework",
                DatabaseName = databaseName,
                ServerName = serverName,
                UserId = userId,
                Password = password
            };
            configuration.ConnectionString = configuration.ConnectionString.Replace("{DBName}", configuration.DatabaseName);
            configuration.ConnectionString = configuration.ConnectionString.Replace("{ServerName}", configuration.ServerName);
            configuration.ConnectionString = configuration.ConnectionString.Replace("{UserId}", configuration.UserId); 
            configuration.ConnectionString = configuration.ConnectionString.Replace("{Password}", configuration.Password);
            return configuration;
        }
    }
}
