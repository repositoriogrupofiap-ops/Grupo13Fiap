using Grupo13Fiap.Application.DTOs.Response;
using Grupo13Fiap.Domain.Entities;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Identity.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grupo13Fiap.WebApi.Controllers.v1;

[Authorize]
public class StoreController(IStoreRepository storeRepository, IGameRepository gameRepository) : ApiControllerBase
{
    private readonly IStoreRepository _storeRepository = storeRepository;
    private readonly IGameRepository _gameRepository = gameRepository;

    /// <summary>
    /// Lista todas as lojas.
    /// </summary>
    /// <response code="200">Lista de lojas retornada com sucesso</response>
    [ProducesResponseType(typeof(IEnumerable<StoreResponse>), StatusCodes.Status200OK)]
    [HttpGet("store")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.GetAllAsync(cancellationToken: cancellationToken);
        return Ok(stores.Select(s => new StoreResponse { Id = s.Id, CreateDate = s.CreateDate }));
    }

    /// <summary>
    /// Retorna uma loja com seus jogos.
    /// </summary>
    /// <response code="200">Loja encontrada</response>
    /// <response code="404">Loja não encontrada</response>
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("store/{storeId:guid}")]
    public async Task<IActionResult> GetById(Guid storeId, CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetWithGamesAsync(storeId, cancellationToken);
        if(store is null)
            return NotFound();

        return Ok(ToResponse(store));
    }

    /// <summary>
    /// Adiciona um jogo à loja.
    /// </summary>
    /// <response code="200">Jogo adicionado com sucesso</response>
    /// <response code="404">Loja ou jogo não encontrado</response>
    /// <response code="409">Jogo já cadastrado na loja</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("store/{storeId:guid}/games/{gameId:guid}")]
    public async Task<IActionResult> AddGame(Guid storeId, Guid gameId, CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetWithGamesAsync(storeId, cancellationToken);
        if(store is null)
            return NotFound("Loja não encontrada.");

        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if(game is null)
            return NotFound("Jogo não encontrado.");

        store.AddGame(game);

        await _storeRepository.UpdateAsync(store, cancellationToken);

        return Ok(ToResponse(store));
    }

    /// <summary>
    /// Remove um jogo da loja.
    /// </summary>
    /// <response code="204">Jogo removido com sucesso</response>
    /// <response code="404">Loja ou jogo não encontrado</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("store/{storeId:guid}/games/{gameId:guid}")]
    public async Task<IActionResult> RemoveGame(Guid storeId, Guid gameId, CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetWithGamesAsync(storeId, cancellationToken);
        if(store is null)
            return NotFound("Loja não encontrada.");

        store.RemoveGame(gameId);

        await _storeRepository.UpdateAsync(store, cancellationToken);

        return NoContent();
    }

    private static StoreResponse ToResponse(Store store) => new()
    {
        Id = store.Id,
        CreateDate = store.CreateDate,
        Games = store.Games.Select(g => new GameResponse
        {
            Id = g.Id,
            Nome = g.Nome,
            Description = g.Description,
            Price = g.Price,
            Category = g.Category,
            DisponibilizationDate = g.DisponibilizationDate,
            IsAvailable = g.IsAvailable(),
            CreateDate = g.CreateDate
        })
    };
}