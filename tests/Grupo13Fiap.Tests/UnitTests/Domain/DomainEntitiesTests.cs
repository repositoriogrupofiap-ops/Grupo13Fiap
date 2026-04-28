using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Enum;
using Grupo13Fiap.Domain.Exceptions;

namespace Grupo13Fiap.Tests.UnitTests.Domain;

public class GameTests
{
    [Fact]
    public void Construtor_DefinePropriedades_E_EstaDisponivel()
    {
        var game = new Game(CategoryGameEnum.Action, "Nome", "Desc", 10m);

        Assert.Equal("Nome", game.Nome);
        Assert.Equal("Desc", game.Description);
        Assert.Equal(10m, game.Price);
        Assert.Equal(CategoryGameEnum.Action, game.Category);
        Assert.True(game.IsAvailable());
    }

    [Fact]
    public void SetNome_Invalido_Lanca()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Game(CategoryGameEnum.RPG, "", "d", 1m));
        Assert.Contains("Nome", ex.Message);
    }

    [Fact]
    public void SetDescricao_Invalida_Lanca()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Game(CategoryGameEnum.RPG, "n", "", 1m));
        Assert.Contains("Descrição", ex.Message);
    }

    [Fact]
    public void SetPreco_Negativo_Lanca()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Game(CategoryGameEnum.RPG, "n", "d", -1m));
        Assert.Contains("Preço", ex.Message);
    }

    [Fact]
    public void AgendarDisponibilizacao_Futuro_TornaIndisponivel()
    {
        var game = new Game(CategoryGameEnum.Adventure, "n", "d", 5m);
        var future = DateTime.UtcNow.AddHours(1);
        game.ScheduleDisponibilization(future);

        Assert.False(game.IsAvailable());
    }

    [Fact]
    public void AgendarDisponibilizacao_Passado_TornaDisponivel()
    {
        var game = new Game(CategoryGameEnum.Adventure, "n", "d", 5m);
        var past = DateTime.UtcNow.AddHours(-1);
        game.ScheduleDisponibilization(past);

        Assert.True(game.IsAvailable());
    }
}

public class LibraryTests
{
    [Fact]
    public void AdicionarJogo_Nulo_Lanca()
    {
        var lib = new Library();
        Assert.Throws<ArgumentNullException>(() => lib.AddGame(null!));
    }

    [Fact]
    public void AdicionarJogo_NaoDisponivel_Lanca()
    {
        var lib = new Library();
        var game = new Game(CategoryGameEnum.Sports, "g", "d", 1m);
        game.ScheduleDisponibilization(DateTime.UtcNow.AddDays(1));

        Assert.Throws<InvalidOperationException>(() => lib.AddGame(game));
    }

    [Fact]
    public void AdicionarJogo_Duplicado_Lanca()
    {
        var lib = new Library();
        var game = new Game(CategoryGameEnum.Sports, "g", "d", 1m);

        lib.AddGame(game);
        Assert.Throws<InvalidOperationException>(() => lib.AddGame(game));
    }

    [Fact]
    public void PossuiJogo_E_ObterJogo_Funciona()
    {
        var lib = new Library();
        var game = new Game(CategoryGameEnum.Strategy, "g", "d", 1m);

        lib.AddGame(game);

        Assert.True(lib.HasGame(game.Id));
        Assert.Equal(game, lib.GetGame(game.Id));
    }
}

public class StoreTests
{
    [Fact]
    public void AdicionarJogo_Nulo_Lanca()
    {
        var store = new Store();
        Assert.Throws<ArgumentNullException>(() => store.AddGame(null!));
    }

    [Fact]
    public void AdicionarJogo_Duplicado_LancaConflictException()
    {
        var store = new Store();
        var game = new Game(CategoryGameEnum.Action, "g", "d", 2m);

        store.AddGame(game);
        Assert.Throws<ConflictException>(() => store.AddGame(game));
    }

    [Fact]
    public void RemoverJogo_NaoEncontrado_LancaNotFoundException()
    {
        var store = new Store();
        var id = Guid.NewGuid();
        Assert.Throws<NotFoundException>(() => store.RemoveGame(id));
    }

    [Fact]
    public void ContemJogo_E_ObterPorId_Funciona()
    {
        var store = new Store();
        var game = new Game(CategoryGameEnum.RPG, "g", "d", 3m);

        store.AddGame(game);

        Assert.True(store.ContainsGame(game.Id));
        Assert.Equal(game, store.GetGameById(game.Id));
    }
}

public class UserTests
{
    [Fact]
    public void Construtor_E_Setters_E_AtribuirBiblioteca_Funciona()
    {
        var user = new User(" Name ", "identity-123");

        Assert.Equal("Name", user.Name);
        Assert.Equal("identity-123", user.IdentityUserId);

        var lib = new Library();
        user.AssignLibrary(lib);

        Assert.Equal(lib, user.Library);
        Assert.Equal(lib.Id, user.LibraryId);
    }

    [Fact]
    public void SetNome_Invalido_Lanca()
    {
        Assert.Throws<ArgumentException>(() => new User("", "id"));
    }

    [Fact]
    public void SetIdentityUserId_Invalido_Lanca()
    {
        Assert.Throws<ArgumentException>(() => new User("n", ""));
    }

    [Fact]
    public void Funcoes_AdicionarRemoverEVerificar_Funciona_E_DuplicadosLancam()
    {
        var user = new User("u", "i");

        user.AddRole(UserRoleEnum.Buyer);
        Assert.True(user.HasRole(UserRoleEnum.Buyer));

        Assert.Throws<InvalidOperationException>(() => user.AddRole(UserRoleEnum.Buyer));

        user.RemoveRole(UserRoleEnum.Buyer);
        Assert.False(user.HasRole(UserRoleEnum.Buyer));

        Assert.Throws<InvalidOperationException>(() => user.RemoveRole(UserRoleEnum.Buyer));
    }

    [Fact]
    public void AtribuirBiblioteca_Nula_Lanca()
    {
        var user = new User("u", "i");
        Assert.Throws<ArgumentNullException>(() => user.AssignLibrary(null!));
    }
}
