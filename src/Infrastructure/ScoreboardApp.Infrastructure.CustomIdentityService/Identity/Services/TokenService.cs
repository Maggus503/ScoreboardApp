﻿using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Errors;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Models;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Options;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Services;
using ScoreboardApp.Infrastructure.CustomIdentityService.Persistence.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScoreboardApp.Infrastructure.Identity.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly TokenSettings _tokenSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenGeneratorService tokenGeneratorService,
            IOptions<TokenSettings> tokenSettings
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGeneratorService = tokenGeneratorService;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<Result<TokenResponse, Error>> Authenticate(AuthenticationRequest request, CancellationToken cancellationToken = default!)
        {
            var signInResult = await SignInUser(request.UserName, request.Password);

            if (signInResult.IsFailure)
            {
                return Result.Failure<TokenResponse, Error>(signInResult.Error);
            }

            ApplicationUser user = signInResult.Value;

            var roles = await _userManager.GetRolesAsync(user);

            string token = await _tokenGeneratorService.GenerateJwtTokenAsync(user, roles);
            string refreshToken = await _tokenGeneratorService.GenerateRefreshTokenAsync();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_tokenSettings.RefreshExpiry);

            await _userManager.UpdateAsync(user);

            return Result.Success<TokenResponse, Error>(new TokenResponse()
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }

        public async Task<Result<TokenResponse, Error>> Refresh(RefreshRequest request, CancellationToken cancellationToken = default!)
        {
            var getClaimsPrincipalResult = GetPrincipalFromExpiredToken(request.Token);

            if (getClaimsPrincipalResult.IsFailure)
            {
                return Result.Failure<TokenResponse, Error>(getClaimsPrincipalResult.Error);
            }

            ClaimsPrincipal principal = getClaimsPrincipalResult.Value;

            ApplicationUser? user = await GetUserByUserName(principal.Identity!.Name!);

            if (user is null)
            {
                return Result.Failure<TokenResponse, Error>(Errors.UserNotFoundError(principal.Identity!.Name!));
            }

            var refreshTokenValidationResult = ValidateRefreshToken(user, request.RefreshToken);

            if (refreshTokenValidationResult.IsFailure)
            {
                return Result.Failure<TokenResponse, Error>(refreshTokenValidationResult.Error);
            }

            var roles = await _userManager.GetRolesAsync(user);

            string token = await _tokenGeneratorService.GenerateJwtTokenAsync(user, roles);
            string refreshToken = await _tokenGeneratorService.GenerateRefreshTokenAsync();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_tokenSettings.RefreshExpiry);

            await _userManager.UpdateAsync(user);

            return Result.Success<TokenResponse, Error>(new TokenResponse()
            {
                Token = token,
                RefreshToken = refreshToken
            });

        }

        public async Task<UnitResult<Error>> Register(RegistrationRequest request, CancellationToken cancellationToken = default!)
        {
            ApplicationUser? existingUser = await GetUserByUserName(request.UserName);

            if (existingUser is not null)
            {
                return UnitResult.Failure(Errors.UserAlreadyExistsError(request.UserName));
            }

            ApplicationUser newUser = new()
            {
                Email = request.Email,
                UserName = request.UserName,
                EmailConfirmed = true // TODO: Add email verification at some point
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);


            return result.Succeeded
                ? UnitResult.Success<Error>()
                : UnitResult.Failure(Errors.RegistrationFailedError().WithDetails(result.Errors.Select(x => x.Description)));
        }

        public async Task<UnitResult<Error>> Revoke(RevokeRequest request, CancellationToken cancellationToken = default!)
        {
            ApplicationUser? user = await GetUserByUserName(request.UserName);

            if (user is null)
            {
                return UnitResult.Failure(Errors.UserNotFoundError(request.UserName));
            }

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Validates the token and returns its ClaimsPrincipal (user).
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns ClaimsPrincipal if input token is valid.</returns>
        private Result<ClaimsPrincipal, Error> GetPrincipalFromExpiredToken(string token)
        {
            byte[] secret = Encoding.ASCII.GetBytes(_tokenSettings.Secret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _tokenSettings.Issuer,
                ValidAudience = _tokenSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                NameClaimType = ClaimTypes.NameIdentifier,
                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) ||
                principal is null)
            {
                return Result.Failure<ClaimsPrincipal, Error>(Errors.InvalidTokenError());
            }

            return Result.Success<ClaimsPrincipal, Error>(principal);

        }

        private async Task<ApplicationUser?> GetUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        /// <summary>
        /// Sign-in user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Returns user object on successful sign in.</returns>
        private async Task<Result<ApplicationUser, Error>> SignInUser(string username, string password)
        {
            ApplicationUser? user = await GetUserByUserName(username);

            if (user == null)
            {
                return Result.Failure<ApplicationUser, Error>(Errors.UserNotFoundError(username));
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, password, true, false);

            if (!signInResult.Succeeded)
            {
                return Result.Failure<ApplicationUser, Error>(Errors.SignInFailedError());
            }

            return Result.Success<ApplicationUser, Error>(user);
        }

        private UnitResult<Error> ValidateRefreshToken(ApplicationUser? user, string refreshToken)
        {
            if (user?.RefreshTokenExpiryTime <= DateTime.UtcNow || user?.RefreshToken != refreshToken)
            {
                return UnitResult.Failure(Errors.InvalidRefreshTokenError());
            }

            return UnitResult.Success<Error>();
        }
    }
}