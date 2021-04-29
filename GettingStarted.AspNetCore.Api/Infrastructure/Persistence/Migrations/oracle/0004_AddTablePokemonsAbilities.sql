CREATE TABLE PokemonAbilities (
    PokemonId nchar(36) NOT NULL,
    AbilityId nchar(36) NOT NULL,
    CONSTRAINT PK_PokemonAbilities PRIMARY KEY (AbilityId, PokemonId),
    CONSTRAINT FK_PokemonAbilitiesAbilityId FOREIGN KEY (AbilityId) REFERENCES Abilities (Id) ON DELETE CASCADE,
    CONSTRAINT FK_PokemonAbilitiesPokemonId FOREIGN KEY (PokemonId) REFERENCES Pokemons (Id) ON DELETE CASCADE
);