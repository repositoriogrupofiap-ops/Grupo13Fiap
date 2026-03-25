using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Infrastructure.Context;

namespace Grupo13Fiap.Tests.Infrastructure;

internal static class TestSeeder
{
    internal const string User1Name   = "Test User One";
    internal const string User2Name   = "Test User Two";
    internal const int    TotalGames  = 4;
    internal const int    TotalUsers  = 2;
    internal const int    GamesUser1  = 2;
    internal const int    GamesUser2  = 2;
    internal const int    TotalStores = 1;
    internal const int    StoreGames  = 4;

    internal static async Task SeedAsync(DBContextGrupo13Fiap context)
    {
        var rpg1    = new Game(CategoryGameEnum.RPG,    "RPG Game One",    "Desc 1", 100m);
        var rpg2    = new Game(CategoryGameEnum.RPG,    "RPG Game Two",    "Desc 2", 200m);
        var action1 = new Game(CategoryGameEnum.Action, "Action Game One", "Desc 3",  50m);
        var action2 = new Game(CategoryGameEnum.Action, "Action Game Two", "Desc 4",  75m);

        rpg1.ScheduleDisponibilization(new DateTime(2021, 1, 1));
        rpg2.ScheduleDisponibilization(new DateTime(2022, 1, 1));
        action1.ScheduleDisponibilization(new DateTime(2023, 1, 1));
        action2.ScheduleDisponibilization(new DateTime(2024, 1, 1));

        var store = new Store();
        store.AddGame(rpg1);
        store.AddGame(rpg2);
        store.AddGame(action1);
        store.AddGame(action2);

        var library1 = new Library();
        library1.AddGame(rpg1);
        library1.AddGame(rpg2);

        var library2 = new Library();
        library2.AddGame(action1);
        library2.AddGame(action2);

        var user1 = new User(User1Name);
        user1.AssignLibrary(library1);

        var user2 = new User(User2Name);
        user2.AssignLibrary(library2);

        await context.Stores.AddAsync(store);
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();
    }
}
