using Dapper.FluentMap.Conventions;
using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api.Domain
{
    [Table("POKEMONS")]
    public class Pokemon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Abilities> Abilities { get; set; }
    }
}
