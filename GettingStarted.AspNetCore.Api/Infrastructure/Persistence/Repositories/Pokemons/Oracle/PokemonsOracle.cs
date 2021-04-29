using Dapper;
using Database.Providers.Connection;
using GettingStarted.AspNetCore.Api.Domain;
using GettingStarted.AspNetCore.Api.Services;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api.Infrastructure.Persistence.Repositories.Pokemons.Oracle
{
    public class PokemonsOracle : IOracleRepository, IPokemonRepository
    {
        private readonly IConfiguration _configuration;
        private readonly PokemonDbContext _pokemonDbContext;

        public PokemonsOracle(IConfiguration configuration, PokemonDbContext pokemonDbContext)
        {
            _configuration = configuration;
            _pokemonDbContext = pokemonDbContext;
        }

        public async Task Create(Pokemon pokemonDto)
        {
            await _pokemonDbContext.Pokemons.AddAsync(pokemonDto);
        }

        public async Task<IEnumerable<Pokemon>> GetPokemonsAsync()
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("Pokemons"));
            var list = await connection.QueryAsync<dynamic>("select * from POKEMONS");
            return list.Select(o => new Pokemon { 
                Id = Guid.Parse(o.ID),
                Email = o.EMAIL,
                Name = o.NAME
            });
        }

        public async Task SaveChangesAsync()
        {
            await _pokemonDbContext.SaveChangesAsync();
        }
    }
}
