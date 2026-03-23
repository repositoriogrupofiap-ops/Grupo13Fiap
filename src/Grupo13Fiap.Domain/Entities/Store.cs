namespace Grupo13Fiap.Domain.Entities;

public class Store : EntityBase
{
    private readonly List<Game> _games = [];

    public IReadOnlyCollection<Game> Games => _games.AsReadOnly();

    public void AddGame(Game game)
    {
        if (game is null)
            throw new ArgumentNullException(nameof(game), "O jogo não pode ser nulo.");

        if (_games.Any(g => g.Id == game.Id))
            throw new InvalidOperationException("O jogo já está cadastrado na loja.");

        _games.Add(game);
    }

    public void RemoveGame(Guid gameId)
    {
        var game = _games.FirstOrDefault(g => g.Id == gameId);

        if (game is null)
            throw new InvalidOperationException("Jogo não encontrado na loja.");

        _games.Remove(game);
    }

    public bool ContainsGame(Guid gameId)
    {
        return _games.Any(g => g.Id == gameId);
    }

    public Game? GetGameById(Guid gameId)
    {
        return _games.FirstOrDefault(g => g.Id == gameId);
    }
}