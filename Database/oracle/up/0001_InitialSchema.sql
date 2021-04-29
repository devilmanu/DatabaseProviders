CREATE TABLE [Pokemons] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL
    CONSTRAINT [PK_Pokemons] PRIMARY KEY ([Id])
);
GO