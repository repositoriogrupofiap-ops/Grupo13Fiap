using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1")]
public class ApiControllerBase : ControllerBase
{
    protected string? GetIdentityUserId()
        => (HttpContext.User.Identity as ClaimsIdentity)
            ?.FindFirst("sub")?.Value;
}