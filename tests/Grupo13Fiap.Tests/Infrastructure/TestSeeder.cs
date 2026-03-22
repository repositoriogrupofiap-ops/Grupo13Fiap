using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Infrastructure.Context;

namespace Grupo13Fiap.Tests.Infrastructure;

internal static class TestSeeder
{
    internal const string User1Name    = "Test User One";
    internal const string User2Name    = "Test User Two";
    internal const int    TotalGames   = 4;
    internal const int    TotalUsers   = 2;
    internal const int    GamesUser1   = 2;
    internal const int    GamesUser2   = 2;
    internal const int    TotalStores  = 1;
    internal const int    StoreGames   = 4;

    internal static async Task SeedAsync(DBContextGrupo13Fiap context)
    {
        var rpg1    = new Game { Nome = "RPG Game One",   Category = CategoryGameEnum.RPG,     Price = 100m, Description = "Desc 1", DisponibilizationDate = new DateTime(2021, 1, 1) };
        var rpg2    = new Game { Nome = "RPG Game Two",   Category = CategoryGameEnum.RPG,     Price = 200m, Description = "Desc 2", DisponibilizationDate = new DateTime(2022, 1, 1) };
        var action1 = new Game { Nome = "Action Game One",Category = CategoryGameEnum.Action,  Price =  50m, Description = "Desc 3", DisponibilizationDate = new DateTime(2023, 1, 1) };
        var action2 = new Game { Nome = "Action Game Two",Category = CategoryGameEnum.Action,  Price =  75m, Description = "Desc 4", DisponibilizationDate = new DateTime(2024, 1, 1) };

        var store    = new Store    { Games = [rpg1, rpg2, action1, action2] };
        var library1 = new Library  { Games = [rpg1, rpg2] };
        var library2 = new Library  { Games = [action1, action2] };

        var user1 = new Users { Name = User1Name, Library = library1, LibraryId = library1.Id };
        var user2 = new Users { Name = User2Name, Library = library2, LibraryId = library2.Id };

        await context.Stores.AddAsync(store);
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();
    }
}
