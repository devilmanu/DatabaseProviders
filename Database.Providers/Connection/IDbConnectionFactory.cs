using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Database.Providers.Connection
{
    public static class DbConnectionFactory
    {
        public const string PROVIDER_KEY = "Provider=";

        public static IDbConnection Create(IConfiguration configuration, string key) 
        {
            var connectionString = configuration.GetConnectionString(key) ?? throw new KeyNotFoundException();
            switch (DiscoverDatabaseType(connectionString))
            {
                case DatabaseType.SQL:
                    return new SqlConnection(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
                case DatabaseType.ORACLE:
                    return new OracleConnection(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
                default:
                    throw new NotSupportedException();
            }
        }

        public static IDbConnection Create(string connectionString)
        {
            switch (DiscoverDatabaseType(connectionString))
            {
                case DatabaseType.SQL:
                    return new SqlConnection(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
                case DatabaseType.ORACLE:
                    return new OracleConnection(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
                default:
                    throw new NotSupportedException();
            }
        }

        public static DatabaseType DiscoverDatabaseType(string connectionString)
        {
            if (connectionString.IndexOf(PROVIDER_KEY) == -1)
                throw new KeyNotFoundException(PROVIDER_KEY);
            var providerSearch = connectionString.IndexOf(PROVIDER_KEY);
            var provider = connectionString[providerSearch..];
            var dbtype = provider[PROVIDER_KEY.Length..];
            DatabaseType type = Enum.TryParse(dbtype.ToUpperInvariant(), out type) ? type : throw new NotSupportedException();
            return type;
        }

        public static IDbConnection Factory(DatabaseType databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case DatabaseType.SQL:
                    return new SqlConnection(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
                case DatabaseType.ORACLE:
                    return new OracleConnection(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public enum DatabaseType
    {
        SQL,
        ORACLE
    }

}
