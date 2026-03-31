using Grupo13Fiap.Application.DTOs.Request;
using Grupo13Fiap.Application.DTOs.Response;
using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Identity.Constants;
using Grupo13Fiap.WebApi.Controllers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grupo13Fiap.WebApi.Controllers.v1;

[Authorize(Roles = Roles.Admin)]
public class GameController : ApiControllerBase
{
    private readonly IGameRepository _gameRepository;

    public GameController(IGameRepository gameRepository) =>
        _gameRepository = gameRepository;

    /// <summary>
    /// Lista todos os jogos.
    /// </summary>
    /// <response code="200">Lista de jogos retornada com sucesso</response>
    [ProducesResponseType(typeof(IEnumerable<GameResponse>), StatusCodes.Status200OK)]
    [HttpGet("games")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var games = await _gameRepository.GetAllAsync(cancellationToken: cancellationToken);
        return Ok(games.Select(ToResponse));
    }

    /// <summary>
    /// Retorna um jogo pelo Id.
    /// </summary>
    /// <response code="200">Jogo encontrado</response>
    /// <response code="404">Jogo não encontrado</response>
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("games/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(id, cancellationToken);
        if (game is null)
            return NotFound();

        return Ok(ToResponse(game));
    }

    /// <summary>
    /// Cria um novo jogo.
    /// </summary>
    /// <response code="201">Jogo criado com sucesso</response>
    /// <response code="400">Retorna erros de validação</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("games")]
    public async Task<IActionResult> Create(GameRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var game = new Game(request.Category, request.Nome, request.Description, request.Price);

        if (request.DisponibilizationDate.HasValue)
            game.ScheduleDisponibilization(request.DisponibilizationDate.Value);

        await _gameRepository.AddAsync(game, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = game.Id }, ToResponse(game));
    }

    /// <summary>
    /// Atualiza um jogo existente.
    /// </summary>
    /// <response code="200">Jogo atualizado com sucesso</response>
    /// <response code="400">Retorna erros de validação</response>
    /// <response code="404">Jogo não encontrado</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("games/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, GameRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var game = await _gameRepository.GetByIdAsync(id, cancellationToken);
        if (game is null)
            return NotFound();

        game.SetNome(request.Nome);
        game.SetDescription(request.Description);
        game.SetPrice(request.Price);
        game.ChangeCategory(request.Category);

        if (request.DisponibilizationDate.HasValue)
            game.ScheduleDisponibilization(request.DisponibilizationDate.Value);

        await _gameRepository.UpdateAsync(game, cancellationToken);

        return Ok(ToResponse(game));
    }

    /// <summary>
    /// Remove um jogo.
    /// </summary>
    /// <response code="204">Jogo removido com sucesso</response>
    /// <response code="404">Jogo não encontrado</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("games/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(id, cancellationToken);
        if (game is null)
            return NotFound();

        await _gameRepository.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    private static GameResponse ToResponse(Game game) => new()
    {
        Id                    = game.Id,
        Nome                  = game.Nome,
        Description           = game.Description,
        Price                 = game.Price,
        Category              = game.Category,
        DisponibilizationDate = game.DisponibilizationDate,
        IsAvailable           = game.IsAvailable(),
        CreateDate            = game.CreateDate
    };
}