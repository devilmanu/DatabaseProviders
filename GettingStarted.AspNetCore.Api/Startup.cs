
using Database.Providers.Connection;
using Database.Providers.EFCore;
using GettingStarted.AspNetCore.Api.Infrastructure.Persistence;
using GettingStarted.AspNetCore.Api.Infrastructure.Persistence.Repositories.Pokemons.Oracle;
using GettingStarted.AspNetCore.Api.Infrastructure.Persistence.Repositories.Pokemons.Sql;
using GettingStarted.AspNetCore.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEFCore<PokemonDbContext>(Configuration.GetConnectionString("Pokemons"), hostEnvironment);


            switch (DbConnectionFactory.DiscoverDatabaseType(Configuration.GetConnectionString("Pokemons")))
            {
                case DatabaseType.SQL:
                    services.AddScoped<IPokemonRepository, PokemonsSql>();
                    break;
                case DatabaseType.ORACLE:
                    services.AddScoped<IPokemonRepository, PokemonsOracle>();
                    break;
                default:
                    throw new NotSupportedException();
            }


            services.AddScoped<IPokemonService, PokemonService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
