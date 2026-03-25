using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(DBContextGrupo13Fiap context)
    {
        if(await context.Users.AnyAsync())
            return;

        var game1 = new Game(CategoryGameEnum.RPG, "Elden Ring", "RPG de mundo aberto da FromSoftware.", 249.90m);
        game1.ScheduleDisponibilization(new DateTime(2022, 2, 25));

        var game2 = new Game(CategoryGameEnum.Action, "God of War Ragnarök", "Aventura épica nórdica com Kratos e Atreus.", 299.90m);
        game2.ScheduleDisponibilization(new DateTime(2022, 11, 9));

        var game3 = new Game(CategoryGameEnum.Adventure, "Hollow Knight", "Metroidvania indie com vasto mundo subterrâneo.", 49.90m);
        game3.ScheduleDisponibilization(new DateTime(2017, 2, 24));

        var game4 = new Game(CategoryGameEnum.RPG, "Baldur's Gate 3", "RPG baseado em D&D com sistema de turnos.", 249.90m);
        game4.ScheduleDisponibilization(new DateTime(2023, 8, 3));

        var game5 = new Game(CategoryGameEnum.Action, "Hades", "Roguelite de ação da Supergiant Games.", 79.90m);
        game5.ScheduleDisponibilization(new DateTime(2020, 9, 17));

        var game6 = new Game(CategoryGameEnum.Simulation, "Stardew Valley", "Simulador de fazenda com elementos RPG.", 39.90m);
        game6.ScheduleDisponibilization(new DateTime(2016, 2, 26));

        var games = new List<Game> { game1, game2, game3, game4, game5, game6 };

        var store = new Store();
        foreach (var game in games)
        {
            store.AddGame(game);
        }

        var library1 = new Library();
        foreach (var game in games.Take(3))
        {
            library1.AddGame(game);
        }

        var library2 = new Library();
        foreach (var game in games.Skip(3))
        {
            library2.AddGame(game);
        }

        var user1 = new User("João Silva");
        user1.AssignLibrary(library1);
        var user2 = new User("Maria Souza");
        user2.AssignLibrary(library2);

        await context.Stores.AddAsync(store);
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();
    }
}
