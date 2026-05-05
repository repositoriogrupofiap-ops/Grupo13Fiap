using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;
using Grupo13Fiap.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grupo13Fiap.Tests.Infrastructure;

public class InfrastructureTests : IAsyncLifetime
{
    private ServiceProvider _provider = null!;

    public async Task InitializeAsync()
    {
        var dbName        = $"TestDb_{Guid.NewGuid()}";
        var services      = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtOptions:SecurityKey"] = "ThisIsATestSecurityKeyForJwtTokenGenerationWithAtLeast32Characters",
                ["JwtOptions:Issuer"] = "TestIssuer",
                ["JwtOptions:Audience"] = "TestAudience",
                ["JwtOptions:AccessTokenExpiration"] = "3600",
                ["JwtOptions:RefreshTokenExpiration"] = "7200"
            })
            .Build();

        services.AddInfrastructure(configuration, options =>
            options.UseInMemoryDatabase(dbName));

        _provider = services.BuildServiceProvider();

        await _provider.InitializeDatabaseAsync();

        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DBContextGrupo13Fiap>();
        await TestSeeder.SeedAsync(db);
    }

    public async Task DisposeAsync() => await _provider.DisposeAsync();

    [Fact]
    public async Task Seed_DevePopularBancoComJogosCorretos()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        var games = await repo.GetAllAsync();

        Assert.Equal(TestSeeder.TotalGames, games.Count());
    }

    [Fact]
    public async Task Seed_DevePopularBancoComUsuariosCorretos()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var users = await repo.GetAllAsync();

        Assert.Equal(TestSeeder.TotalUsers, users.Count());
    }

    [Fact]
    public async Task UsersRepository_DeveBuscarUsuarioPorNome()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var user = await repo.GetByNameAsync(TestSeeder.User1Name);

        Assert.NotNull(user);
        Assert.Equal(TestSeeder.User1Name, user.Name);
    }

    [Fact]
    public async Task UsersRepository_DeveRetornarUsuarioComLibraryEGames()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var users           = await repo.GetAllAsync();
        var userWithLibrary = await repo.GetWithLibraryAsync(users.First().Id);

        Assert.NotNull(userWithLibrary);
        Assert.NotNull(userWithLibrary.Library);
        Assert.Equal(TestSeeder.GamesUser1, userWithLibrary.Library.Games.Count);
    }

    [Fact]
    public async Task GameRepository_DeveRetornarJogosPorCategoria()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        var rpgGames = await repo.GetByCategoryAsync(CategoryGameEnum.RPG);

        Assert.NotEmpty(rpgGames);
        Assert.All(rpgGames, g => Assert.Equal(CategoryGameEnum.RPG, g.Category));
    }

    [Fact]
    public async Task StoreRepository_DeveRetornarLojaComTodosOsJogos()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IStoreRepository>();

        var stores         = await repo.GetAllAsync();
        var storeWithGames = await repo.GetWithGamesAsync(stores.First().Id);

        Assert.NotNull(storeWithGames);
        Assert.Equal(TestSeeder.StoreGames, storeWithGames.Games.Count);
    }
}

