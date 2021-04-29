using Database.Providers.Connection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Providers.EFCore
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection AddEFCore<TDbContext>(this IServiceCollection services, string connectionstring, IHostEnvironment hostEnvironment) where TDbContext : DbContext
        {
            switch (DbConnectionFactory.DiscoverDatabaseType(connectionstring))
            {
                case DatabaseType.SQL:
                    services.AddDbContext<TDbContext>(options => options.UseSqlServer(connectionstring).SetupSensitiveLogging(hostEnvironment));
                    break;
                case DatabaseType.ORACLE:
                    services.AddDbContext<TDbContext>(options => options.UseOracle(connectionstring).SetupSensitiveLogging(hostEnvironment));
                    break;
                default:
                    throw new NotSupportedException();
            }
            return services;
        }

        private static DbContextOptionsBuilder SetupSensitiveLogging(this DbContextOptionsBuilder optionsBuilder, IHostEnvironment environment)
        {
            return optionsBuilder.EnableDetailedErrors()
                    .EnableSensitiveDataLogging(environment.IsDevelopment());
        }
    }
}
