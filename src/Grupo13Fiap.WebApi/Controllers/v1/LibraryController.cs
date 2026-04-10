using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grupo13Fiap.Application.DTOs.Response;
using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grupo13Fiap.WebApi.Controllers.v1;

[Authorize]
public class LibraryController : ApiControllerBase
{
    private readonly IUsersRepository   _usersRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IStoreRepository   _storeRepository;

    public LibraryController(
        IUsersRepository   usersRepository,
        ILibraryRepository libraryRepository,
        IStoreRepository   storeRepository)
    {
        _usersRepository   = usersRepository;
        _libraryRepository = libraryRepository;
        _storeRepository   = storeRepository;
    }

    /// <summary>
    /// Retorna a biblioteca de jogos do usuário autenticado.
    /// </summary>
    /// <response code="200">Biblioteca retornada com sucesso</response>
    /// <response code="404">Usuário não encontrado</response>
    [ProducesResponseType(typeof(LibraryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("library")]
    public async Task<IActionResult> GetMyLibrary(CancellationToken cancellationToken)
    {
        var identityUserId = GetIdentityUserId();
        if (identityUserId is null)
            return Unauthorized();

        var user = await _usersRepository.GetWithLibraryByIdentityUserIdAsync(identityUserId, cancellationToken);
        if (user is null)
            return NotFound("Usuário não encontrado.");

        if (user.Library is null)
            return Ok(new LibraryResponse());

        return Ok(ToResponse(user.Library));    
    }

    /// <summary>
    /// Adiciona um jogo da loja à biblioteca do usuário autenticado.
    /// </summary>
    /// <response code="200">Jogo adicionado com sucesso</response>
    /// <response code="400">Jogo ainda não disponível</response>
    /// <response code="404">Loja, jogo ou usuário não encontrado</response>
    /// <response code="409">Jogo já está na biblioteca</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(LibraryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("library/games/{gameId:guid}")]
    public async Task<IActionResult> AddGame(Guid gameId, [FromQuery] Guid storeId, CancellationToken cancellationToken)
    {
        var identityUserId = GetIdentityUserId();
        if (identityUserId is null)
            return Unauthorized();

        var user = await _usersRepository.GetWithLibraryByIdentityUserIdAsync(identityUserId, cancellationToken);
        if (user is null)
            return NotFound("Usuário não encontrado.");

        if (user.Library is null)
            return BadRequest("Usuário não possui uma biblioteca.");

        var store = await _storeRepository.GetWithGamesAsync(storeId, cancellationToken);
        if (store is null)
            return NotFound("Loja não encontrada.");

        var game = store.GetGameById(gameId);
        if (game is null)
            return NotFound("Jogo não encontrado na loja.");

        if (!game.IsAvailable())
            return BadRequest("O jogo ainda não está disponível.");

        if (user.Library.HasGame(gameId))
            return Conflict("O jogo já está na biblioteca.");

        user.Library.AddGame(game);
        await _libraryRepository.UpdateAsync(user.Library, cancellationToken);

        return Ok(ToResponse(user.Library));
    }

    private string? GetIdentityUserId()
        => (HttpContext.User.Identity as ClaimsIdentity)
            ?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    private static LibraryResponse ToResponse(Library library) => new()
    {
        Id         = library.Id,
        CreateDate = library.CreateDate,
        Games      = library.Games.Select(g => new GameResponse
        {
            Id                    = g.Id,
            Nome                  = g.Nome,
            Description           = g.Description,
            Price                 = g.Price,
            Category              = g.Category,
            DisponibilizationDate = g.DisponibilizationDate,
            IsAvailable           = g.IsAvailable(),
            CreateDate            = g.CreateDate
        })
    };
}