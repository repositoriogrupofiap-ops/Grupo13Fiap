namespace Grupo13Fiap.Application.DTOs.Response;

public class LibraryResponse
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public IEnumerable<GameResponse> Games { get; set; } = [];
}