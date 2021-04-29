using GettingStarted.AspNetCore.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api.Services
{
    public interface IPokemonRepository
    {
        Task Create(Pokemon pokemonDto);
        Task<IEnumerable<Pokemon>> GetPokemonsAsync();
        Task SaveChangesAsync();
    }
}
