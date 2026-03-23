namespace Grupo13Fiap.Domain.Entities;

public class Library : EntityBase
{
    private readonly List<Game> _games = [];

    public IReadOnlyCollection<Game> Games => _games.AsReadOnly();

    public void AddGame(Game game)
    {
        if (game is null)
            throw new ArgumentNullException(nameof(game), "O jogo não pode ser nulo.");

        if (!game.IsAvailable())
            throw new InvalidOperationException("O jogo ainda não está disponível.");

        if (_games.Any(g => g.Id == game.Id))
            throw new InvalidOperationException("O jogo já está na biblioteca.");

        _games.Add(game);
    }

    public bool HasGame(Guid gameId)
    {
        return _games.Any(g => g.Id == gameId);
    }

    public Game? GetGame(Guid gameId)
    {
        return _games.FirstOrDefault(g => g.Id == gameId);
    }
}