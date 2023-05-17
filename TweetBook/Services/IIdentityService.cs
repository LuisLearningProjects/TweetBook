﻿using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
