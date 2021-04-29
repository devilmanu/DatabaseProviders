using GettingStarted.AspNetCore.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GettingStarted.AspNetCore.Api.Infrastructure.Persistence
{
    public class PokemonDbContext : DbContext
    {
        public PokemonDbContext(DbContextOptions<PokemonDbContext> optionsBuilder) : base(optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<string, Guid>(
              v => new Guid(v),
              v => v.ToString());

            modelBuilder.Entity<Pokemon>().Property(b => b.Id).HasColumnName("ID");
            modelBuilder.Entity<Pokemon>().Property(b => b.Name).HasColumnName("NAME");
            modelBuilder.Entity<Pokemon>().Property(b => b.Email).HasColumnName("EMAIL");
        }


        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<Abilities> Abilities { get; set; }
    }
}
