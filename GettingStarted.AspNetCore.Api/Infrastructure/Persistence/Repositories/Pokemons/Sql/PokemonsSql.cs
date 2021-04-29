using Dapper;
using Database.Providers.Connection;
using GettingStarted.AspNetCore.Api.Domain;
using GettingStarted.AspNetCore.Api.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api.Infrastructure.Persistence.Repositories.Pokemons.Sql
{
    public class PokemonsSql : ISQLRepository, IPokemonRepository
    {
        private readonly IConfiguration _configuration;
        private readonly PokemonDbContext _pokemonDbContext;

        public PokemonsSql(IConfiguration configuration, PokemonDbContext pokemonDbContext)
        {
            _configuration = configuration;
            _pokemonDbContext = pokemonDbContext;
        }

        public PokemonDbContext PokemonDbContext { get; }

        public async Task Create(Pokemon pokemonDto)
        {
            await _pokemonDbContext.Pokemons.AddAsync(pokemonDto);
        }

        public async Task<IEnumerable<Pokemon>> GetPokemonsAsync()
        {
            using(var connection = new SqlConnection(_configuration.GetConnectionString("Pokemons")))
            {
                try
                {
                    var kk = await connection.QueryAsync<Pokemon>("select * from pokemons");
                }
                catch (Exception ex)
                {

                }
                return await connection.QueryAsync<Pokemon>("select * from pokemons");
            };
        }

        public async Task SaveChangesAsync()
        {
            await _pokemonDbContext.SaveChangesAsync();
        }
    }
}
