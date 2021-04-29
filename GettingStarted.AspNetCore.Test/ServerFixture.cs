using Dapper;
using Database.Providers.Connection;
using DbUp;
using DbUp.Engine;
using DbUp.Oracle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;
using RunProcessAsTask;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GettingStarted.AspNetCore.Test
{
    public static class RoundHouseHelpers
    {
        /// <summary>
        /// -f {databaseFolderPath} --databasename=DB-Test --connectionstring=\"{connectionString}\" --silent -t --env TEST
        /// </summary>
        /// <param name="databaseFolderPath"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DatabaseUpgradeResult MigrateSql(string databaseFolderPath, string connectionString, string databaseName, string databaseType = "sqlserver") {
            EnsureDatabase.For.SqlDatabase(connectionString);
            return DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(databaseFolderPath)
            .WithTransaction()
            .LogToConsole()
            .Build().PerformUpgrade();
        }

        public static DatabaseUpgradeResult MigrateOracle(string databaseFolderPath, string connectionString, string databaseName, string databaseType = "sqlserver")
        {
            var kk = DeployChanges.To
                .OracleDatabase(connectionString, ';')
                .WithScriptsFromFileSystem(databaseFolderPath)
                .LogToConsole()
                .Build();
            var scrips = kk.GetScriptsToExecute();
            return kk.PerformUpgrade();
        }

        public static Task<ProcessResults> RhCommand(string arguments) => ProcessEx.RunAsync("rh", arguments);

        public static Task ResetDatabase(IDbConnection connection) => DeleteTablesDatabase(connection);

        public static Task ResetDatabaseOracle(IDbConnection connection) => DeleteTablesDatabaseOracle(connection);
        public static Task DeleteTablesDatabase(IDbConnection connection)
        {
            //elimina todas las tablas de la bbdd
            var dropQuery = @"DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR
            SET @Cursor = CURSOR FAST_FORWARD FOR
            SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_SCHEMA + '].[' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + '];'
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
            LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME = rc1.CONSTRAINT_NAME
            OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql
            WHILE(@@FETCH_STATUS = 0)
            BEGIN
            Exec sp_executesql @Sql
            FETCH NEXT FROM @Cursor INTO @Sql
            END
            CLOSE @Cursor DEALLOCATE @Cursor
            EXEC sp_MSforeachtable 'DROP TABLE ?'".Replace(System.Environment.NewLine, string.Empty);
            connection.Query(dropQuery);
            return Task.CompletedTask;


        }

        public static Task DeleteTablesDatabaseOracle(IDbConnection connection)
        {
            //elimina todas las tablas de la bbdd
            var dropQuery = @"begin
  for t in (select table_name  from user_tables)
  loop 
    execute immediate ' truncate '||t.table_name; 
  end loop; 
end;".Replace(System.Environment.NewLine, string.Empty);
            connection.Query(dropQuery);
            return Task.CompletedTask;

        }
    }

    public class ServerFixture<TTestStartup> where TTestStartup : class
    {
        public TestServer TestServer { get; private set; }

        private readonly IConfiguration _configuration;

        private static IHost _host;
        private static string _databaseFolderPath;
        private static string _connectionString;
        private static string _databaseName;

        public ServerFixture()
        {
            InitializeTestServer();
        }

        private void InitializeTestServer() 
        {
            _host = new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder
                    .ConfigureServices(services => services.AddSingleton<IServer>(serviceProvider => new TestServer(serviceProvider)))
                    .UseStartup<TTestStartup>();
                })
                .ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    var configuration = configurationBuilder.Build();
                }).Build();

            _host.StartAsync().Wait();

            RoundHouseHelpers.MigrateSql(_databaseFolderPath, _databaseName, _connectionString);

            TestServer = _host.GetTestServer();
        }

        public static Task DeleteTablesDatabase(string connectionString)
        {
            //elimina todas las tablas de la bbdd
            var dropQuery = @"DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR
            SET @Cursor = CURSOR FAST_FORWARD FOR
            SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_SCHEMA + '].[' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + '];'
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
            LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME = rc1.CONSTRAINT_NAME
            OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql
            WHILE(@@FETCH_STATUS = 0)
            BEGIN
            Exec sp_executesql @Sql
            FETCH NEXT FROM @Cursor INTO @Sql
            END
            CLOSE @Cursor DEALLOCATE @Cursor
            EXEC sp_MSforeachtable 'DROP TABLE ?'".Replace(System.Environment.NewLine, string.Empty);
            var start = connectionString.IndexOf("Provider=");
            var dbtype = connectionString.Substring(start, _connectionString.Length);
            IDbConnection dbconnection;
            switch (Enum.Parse<DatabaseType>(dbtype))
            {
                case DatabaseType.SQL:
                    dbconnection = new SqlConnection(_connectionString);
                    break;
                case DatabaseType.ORACLE:
                    dbconnection = new OracleConnection(_connectionString);
                    break;
                default:
                    throw new NotSupportedException();
            }
            dbconnection.Query(dropQuery);
            return Task.CompletedTask;

        }
    }

    public class ServerFixture
    {
        public TestServer TestServer { get; set; }

        public ServerFixture(IConfiguration configuration, Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configureApp)
        {
            var host = new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.ConfigureServices(configureServices);
                    builder.ConfigureServices(services => services.AddSingleton<IServer>(serviceProvider => new TestServer(serviceProvider)));
                    builder.Configure(configureApp);
                })
                .ConfigureAppConfiguration((_, cfg) =>
                {
                    cfg.AddConfiguration(configuration);
                }).Build();

            host.StartAsync().Wait();

            TestServer = host.GetTestServer();
        }
    }

    [CollectionDefinition(nameof(AspNetCoreServer<TTestStartup>))]
    public class AspNetCoreServer<TTestStartup> 
    : ICollectionFixture<ServerFixture<TTestStartup>> where TTestStartup : class
    {

    }


}
