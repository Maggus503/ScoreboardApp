﻿using AutoMapper;
using FluentValidation.Results;
using MediatR;
using ScoreboardApp.Application.Commons.Exceptions;
using ScoreboardApp.Application.Commons.Mappings;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Models;
using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Services;
using ScoreboardApp.Infrastructure.CustomIdentityService.Persistence.Entities;

namespace ScoreboardApp.Application.Authentication
{
    public sealed record RefreshCommand() : IRequest<RefreshCommandResponse>
    {
        public string Token { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
    }
    public sealed record RefreshCommandResponse() : IMapFrom<TokenResponse>
    {
        public string Token { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
    }

    public sealed class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshCommandResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RefreshCommandHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<RefreshCommandResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            var principal = await _userService.GetPrincipalFromTokenAsync(request.Token);

            if (principal is null)
            {
                throw new ValidationException();
            }

            string username = principal!.Identity!.Name!;
            ApplicationUser? user = await _userService.GetUserByUserNameAsync(username);

            if (user is null)
            {
                throw new NotFoundException("user", username);
            }

            var refreshTokenValidationResult = _userService.ValidateRefreshToken(user, request.RefreshToken);

            if (refreshTokenValidationResult.IsFailure)
            {
                throw new ValidationException();
            }

            var tokenResponse = await _userService.GenerateTokensForUserAsync(user);

            return _mapper.Map<RefreshCommandResponse>(tokenResponse);
        }
    }
}