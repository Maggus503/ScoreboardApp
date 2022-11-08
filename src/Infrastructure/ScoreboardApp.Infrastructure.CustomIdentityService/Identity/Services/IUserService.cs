﻿using CSharpFunctionalExtensions;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Models;
using ScoreboardApp.Infrastructure.CustomIdentityService.Persistence.Entities;
using System.Security.Claims;

namespace ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByUserNameAsync(string userName);
        Task<Result> SignInUserAsync(ApplicationUser user, string password);
        Task<TokenResponse> GenerateTokensForUserAsync(ApplicationUser user);
        Result ValidateRefreshToken(ApplicationUser? user, string refreshToken);
        Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string jwtToken);
        Task<Result<string, Error>> CreateUserAsync(ApplicationUser newUser, string password);
        Task RevokeUsersRefreshTokenAsync(ApplicationUser user);
    }
}