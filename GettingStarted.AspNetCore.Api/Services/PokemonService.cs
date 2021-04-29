using GettingStarted.AspNetCore.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api.Services
{
    public interface IPokemonService 
    {
        Task<Pokemon> CreatePokemon(PokemonDto pokemonDto);
        Task<IEnumerable<PokemonDto>> GetPokemons();
    }
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonRepository _pokemonRepository;

        public PokemonService(IPokemonRepository pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;
        }

        public async Task<Pokemon> CreatePokemon(PokemonDto pokemonDto)
        {
            var newPokemon = new Pokemon
            {
                Id = pokemonDto.Id,
                Name = pokemonDto.Name
            };
            await _pokemonRepository.Create(newPokemon);
            return newPokemon;
        }

        public async Task<IEnumerable<PokemonDto>> GetPokemons()
        {
            var pokemons = await _pokemonRepository.GetPokemonsAsync();
            return pokemons.Select(p =>
                new PokemonDto
                {
                    Id = p.Id,
                    Name = p.Name
                }) ;
        }
    }
}
