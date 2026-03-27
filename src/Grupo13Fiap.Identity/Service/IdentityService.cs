using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Grupo13Fiap.Application.DTOs.Request;
using Grupo13Fiap.Application.DTOs.Response;
using Grupo13Fiap.Application.Interfaces.Services;
using Grupo13Fiap.Identity.Configurations;

namespace Grupo13Fiap.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               IOptions<JwtOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<UserRegistrationResponse> RegisterUser(UserRegistrationRequest userRegistration)
        {
            var identityUser = new IdentityUser
            {
                UserName = userRegistration.Email,
                Email = userRegistration.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, userRegistration.Password);
            if (result.Succeeded)
                await _userManager.SetLockoutEnabledAsync(identityUser, false);

            var userRegistrationResponse = new UserRegistrationResponse(result.Succeeded);
            if (!result.Succeeded && result.Errors.Count() > 0)
                userRegistrationResponse.AddErrors(result.Errors.Select(r => r.Description));

            return userRegistrationResponse;
        }

        public async Task<UserLoginResponse> Login(UserLoginRequest userLogin)
        {
            var result = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);
            if (result.Succeeded)
                return await GenerateCredentials(userLogin.Email);

            var userLoginResponse = new UserLoginResponse();
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    userLoginResponse.AddError("Essa conta está bloqueada");
                else if (result.IsNotAllowed)
                    userLoginResponse.AddError("Essa conta não tem permissão para fazer login");
                else if (result.RequiresTwoFactor)
                    userLoginResponse.AddError("É necessário confirmar o login no seu segundo fator de autenticação");
                else
                    userLoginResponse.AddError("Usuário ou senha estão incorretos");
            }

            return userLoginResponse;
        }

        public async Task<UserLoginResponse> LoginWithoutPassword(string usuarioId)
        {
            var userLoginResponse = new UserLoginResponse();
            var user = await _userManager.FindByIdAsync(usuarioId);
            
            if (await _userManager.IsLockedOutAsync(user))
                userLoginResponse.AddError("Essa conta está bloqueada");
            else if (!await _userManager.IsEmailConfirmedAsync(user))
                userLoginResponse.AddError("Essa conta precisa confirmar seu e-mail antes de realizar o login");
            
            if (userLoginResponse.Success)
                return await GenerateCredentials(user.Email);

            return userLoginResponse;
        }

        private async Task<UserLoginResponse> GenerateCredentials(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await GetClaims(user, addClaimsUser: true);
            var refreshTokenClaims = await GetClaims(user, addClaimsUser: false);

            var dateTokenExpiration = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dateRefreshTokenExpiration = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = GenerateToken(accessTokenClaims, dateTokenExpiration);
            var refreshToken = GenerateToken(refreshTokenClaims, dateRefreshTokenExpiration);
            return new UserLoginResponse
            (
                success: true,
                accessToken: accessToken,
                refreshToken: refreshToken
            );
        }

        private string GenerateToken(IEnumerable<Claim> claims, DateTime dateExpiration)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dateExpiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<IList<Claim>> GetClaims(IdentityUser user, bool addClaimsUser)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

            if (addClaimsUser)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                claims.AddRange(userClaims);

                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }

            return claims;
        }
    }
}