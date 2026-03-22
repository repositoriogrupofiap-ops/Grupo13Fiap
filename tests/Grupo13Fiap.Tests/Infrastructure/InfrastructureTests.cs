using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grupo13Fiap.Tests.Infrastructure;

public class InfrastructureTests : IAsyncLifetime
{
    private ServiceProvider _provider = null!;

    public async Task InitializeAsync()
    {
        var services      = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddInfrastructure(configuration);

        _provider = services.BuildServiceProvider();

        await _provider.InitializeDatabaseAsync();
    }

    public async Task DisposeAsync() => await _provider.DisposeAsync();

    [Fact]
    public async Task Seed_DevePopularBancoComSeiJogos()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        var games = await repo.GetAllAsync();

        Assert.Equal(6, games.Count());
    }

    [Fact]
    public async Task Seed_DevePopularBancoComDoisUsuarios()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var users = await repo.GetAllAsync();

        Assert.Equal(2, users.Count());
    }

    [Fact]
    public async Task UsersRepository_DeveBuscarUsuarioPorNome()
    {
        using var scope = _provider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var user = await repo.GetByNameAsync("João Silva");

        Assert.NotNull(user);
        Assert.Equal("João Silva", user.Name);
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
        Assert.NotEmpty(userWithLibrary.Library.Games);
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
        Assert.Equal(6, storeWithGames.Games.Count);
    }
}
