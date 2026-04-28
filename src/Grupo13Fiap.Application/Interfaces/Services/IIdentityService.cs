using Grupo13Fiap.Application.DTOs.Request;
using Grupo13Fiap.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo13Fiap.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<UserRegistrationResponse> RegisterUser(UserRegistrationRequest userRegistration);
    Task<UserLoginResponse> Login(UserLoginRequest userLogin);
    Task<UserLoginResponse> LoginWithoutPassword(string userId);
}