﻿using ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Models;

namespace ScoreboardApp.Infrastructure.CustomIdentityService.Identity.Errors
{
    public static class Errors
    {
        public static readonly Error UserNotFoundError = new() { Code = nameof(UserNotFoundError), Message = "User with given username does not exist." , StatusCode = 400};
        
        public static readonly Error SignInFailedError = new() { Code = nameof(SignInFailedError), Message = "Error occured while trying to sign in. Validate user credentials.", StatusCode = 400 };

        public static readonly Error InvalidTokenError = new() { Code = nameof(InvalidTokenError), Message = "Token is not valid." , StatusCode = 400 };

        public static readonly Error InvalidRefreshTokenError = new() { Code = nameof(InvalidRefreshTokenError), Message = "Refresh token is not valid", StatusCode = 400 };

        public static readonly Error UserAlreadyExistsError = new() { Code = nameof(UserAlreadyExistsError), Message = "User with given username already exists.", StatusCode = 409 };

        public static readonly Error RegistrationFailedError = new() { Code = nameof(RegistrationFailedError), Message = "Error occured while trying to create new user." , StatusCode = 400 };
    }
}                   