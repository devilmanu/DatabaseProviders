using GettingStarted.AspNetCore.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GettingStarted.AspNetCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonsController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonsController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        // GET: api/<PokemonsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _pokemonService.GetPokemons());
        }

        // POST api/<PokemonsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PokemonDto value)
        {
            await _pokemonService.CreatePokemon(value);
            return Created("",value);
        }
    }
}
