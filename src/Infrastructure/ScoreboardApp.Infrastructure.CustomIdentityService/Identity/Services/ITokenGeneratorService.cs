﻿using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Options;
using ScoreboardApp.Infrastructure.CustomIdentityService.Persistence.Entities;

namespace ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Services
{
    public interface ITokenGeneratorService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles);
        Task<string> GenerateRefreshTokenAsync();
    }
}