using System.Net;
using System.Security.Claims;
using Grupo13Fiap.Application.DTOs.Response;
using Grupo13Fiap.Application.Interfaces.Services;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Grupo13Fiap.WebApi.Controllers.Shared;
using Grupo13Fiap.Application.DTOs.Request;
namespace Grupo13Fiap.WebApi.Controllers.v1;

public class UserController : ApiControllerBase
{
    private readonly IIdentityService  _identityService;
    private readonly IUsersRepository  _usersRepository;

    public UserController(IIdentityService identityService, IUsersRepository usersRepository)
    {
        _identityService = identityService;
        _usersRepository = usersRepository;
    }

    /// <summary>
    /// Cadastro de usuário.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="userRegistration">Dados de cadastro do usuário</param>
    /// <returns></returns>
    /// <response code="200">Usuário criado com sucesso</response>
    /// <response code="400">Retorna erros de validação</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(UserRegistrationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("user/register")]
    public async Task<IActionResult> Register(UserRegistrationRequest userRegister)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var result = await _identityService.RegisterUser(userRegister);

        if (!result.Success)
        {
            if (result.Erros.Count > 0)
                return BadRequest(new CustomProblemDetails(HttpStatusCode.BadRequest, Request, errors: result.Erros));

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var domainUser = new User(userRegister.Name, result.IdentityUserId!);
        await _usersRepository.AddAsync(domainUser);

        return Ok(result);
    }

    /// <summary>
    /// Login do usuário via usuário/senha.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="userLogin">Dados de login do usuário</param>
    /// <returns></returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Retorna erros de validação</response>
    /// <response code="401">Erro caso usuário não esteja autorizado</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("user/login")]
    public async Task<ActionResult<UserLoginResponse>> Login(UserLoginRequest userLogin)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var resultado = await _identityService.Login(userLogin);
        if (resultado.Success)
            return Ok(resultado);

        return Unauthorized();
    }

    /// <summary>
    /// Login do usuário via refresh token.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Retorna erros de validação</response>
    /// <response code="401">Erro caso usuário não esteja autorizado</response>
    /// <response code="500">Retorna erros caso ocorram</response>
    [ProducesResponseType(typeof(UserLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Authorize]
    [HttpPost("user/refresh-login")]
    public async Task<ActionResult<UserLoginResponse>> RefreshLogin()
    {
        var identity  = HttpContext.User.Identity as ClaimsIdentity;
        var usuarioId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioId == null)
            return BadRequest();

        var resultado = await _identityService.LoginWithoutPassword(usuarioId);
        if (resultado.Success)
            return Ok(resultado);

        return Unauthorized();
    }
}