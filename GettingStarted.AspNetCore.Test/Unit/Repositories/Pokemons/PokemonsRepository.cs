using GettingStarted.AspNetCore.Api.Infrastructure.Persistence;
using GettingStarted.AspNetCore.Api.Infrastructure.Persistence.Repositories.Pokemons.Oracle;
using GettingStarted.AspNetCore.Api.Infrastructure.Persistence.Repositories.Pokemons.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GettingStarted.AspNetCore.Test.Unit.Repositories.Pokemons
{
    public class PokemonsRepository
    {
        public PokemonsRepository()
        {

        }

        [Fact]
        public async Task TestData()
        {
            //await RoundHouseHelpers.ResetDatabase(new SqlConnection("Server=.;Database=pokemons-test;User Id=sa;Password=Password_123;MultipleActiveResultSets=true"));
            var result = RoundHouseHelpers.MigrateSql(@"C:\Users\devil\source\repos\DatabaseProviders\GettingStarted.AspNetCore.Api\Infrastructure\Persistence\Migrations\sql", "Server=.;Database=pokemons-test;User Id=sa;Password=Password_123;MultipleActiveResultSets=true", "pokemons-test");
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>{
                    {"ConnectionStrings:Pokemons", "Server=.;Database=pokemons-test;User Id=sa;Password=Password_123;MultipleActiveResultSets=true" },
                }
            ).Build();

            var options = new DbContextOptionsBuilder<PokemonDbContext>();
            options.UseSqlServer("Server=.;Database=pokemons-test;User Id=sa;Password=Password_123;MultipleActiveResultSets=true");
            var dbContext = new PokemonDbContext(options.Options);
            var repo = new PokemonsSql(configuration, dbContext);

            var pokemon = new Api.Domain.Pokemon
            {
                Id = new Guid("1eb892bb-69a0-4677-b093-843f713fade9"),
                Name = "paco"
            };
            await repo.Create(pokemon);
            await repo.SaveChangesAsync();


            var data = await repo.GetPokemonsAsync();
            Assert.NotNull(data);
            await RoundHouseHelpers.ResetDatabase(new SqlConnection("Server=.;Database=pokemons-test;User Id=sa;Password=Password_123;MultipleActiveResultSets=true"));
        }


        [Fact]
        public async Task Oracle()
        {
            const string ORACLE = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xe)));User Id=system;Password=2e16b6823be300a9;";
            //try
            //{
            //    await RoundHouseHelpers.ResetDatabaseOracle(new OracleConnection(ORACLE));
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

            var result = RoundHouseHelpers.MigrateOracle(@"C:\Users\devil\source\repos\DatabaseProviders\GettingStarted.AspNetCore.Api\Infrastructure\Persistence\Migrations\oracle", ORACLE, "pokemons-test", "roundhouse.databases.oracle");
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>{
                    {"ConnectionStrings:Pokemons", ORACLE },
                }
            ).Build();

            var options = new DbContextOptionsBuilder<PokemonDbContext>();
            var mb = new ModelBuilder(new ConventionSet() { }).Model;
            options.UseOracle(ORACLE, a =>
            {
                a.UseOracleSQLCompatibility("11");
            });
            options.EnableSensitiveDataLogging();
            //options.UseModel(mb);

            var dbContext = new PokemonDbContext(options.Options);
            var repo = new PokemonsOracle(configuration, dbContext);
            //var pokemon = new Api.Domain.Pokemon
            //{
            //    Id = new Guid("1eb892bb-69a0-4677-b093-843f713fade9"),
            //    Name = "paco"
            //};
            //await repo.Create(pokemon);
            //await repo.SaveChangesAsync();


            var data = await repo.GetPokemonsAsync();
            await RoundHouseHelpers.ResetDatabase(new OracleConnection(ORACLE));
        }
    }
}
