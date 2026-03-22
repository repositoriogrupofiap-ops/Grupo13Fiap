using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(DBContextGrupo13Fiap context)
    {
        if (await context.Users.AnyAsync())
            return;

        var games = new List<Game>
        {
            new() { Nome = "Elden Ring",        Category = CategoryGameEnum.RPG,        Price = 249.90m, Description = "RPG de mundo aberto da FromSoftware.",           DisponibilizationDate = new DateTime(2022, 2,  25) },
            new() { Nome = "God of War Ragnarök",Category = CategoryGameEnum.Action,     Price = 299.90m, Description = "Aventura épica nórdica com Kratos e Atreus.",    DisponibilizationDate = new DateTime(2022, 11,  9) },
            new() { Nome = "Hollow Knight",      Category = CategoryGameEnum.Adventure,  Price =  49.90m, Description = "Metroidvania indie com vasto mundo subterrâneo.", DisponibilizationDate = new DateTime(2017, 2,  24) },
            new() { Nome = "Baldur's Gate 3",    Category = CategoryGameEnum.RPG,        Price = 249.90m, Description = "RPG baseado em D&D com sistema de turnos.",       DisponibilizationDate = new DateTime(2023, 8,   3) },
            new() { Nome = "Hades",              Category = CategoryGameEnum.Action,     Price =  79.90m, Description = "Roguelite de ação da Supergiant Games.",          DisponibilizationDate = new DateTime(2020, 9,  17) },
            new() { Nome = "Stardew Valley",     Category = CategoryGameEnum.Simulation, Price =  39.90m, Description = "Simulador de fazenda com elementos RPG.",         DisponibilizationDate = new DateTime(2016, 2,  26) },
        };

        var store = new Store { Games = [.. games] };

        var library1 = new Library { Games = games.Take(3).ToList() };
        var library2 = new Library { Games = games.Skip(3).ToList() };

        var user1 = new Users { Name = "João Silva",  Library = library1, LibraryId = library1.Id };
        var user2 = new Users { Name = "Maria Souza", Library = library2, LibraryId = library2.Id };

        await context.Stores.AddAsync(store);
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();
    }
}
