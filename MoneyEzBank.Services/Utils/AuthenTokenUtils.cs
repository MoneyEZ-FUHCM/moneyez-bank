﻿using Microsoft.Extensions.Configuration;
using MoneyEzBank.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Utils
{
    public static class AuthenTokenUtils
    {
        public static string GenerateAccessToken(string email, User user, IConfiguration configuration)
        {
            var role = user.Role.ToString();

            var authClaims = new List<Claim>();

            if (role != null)
            {
                authClaims.Add(new Claim(ClaimTypes.Email, email));
                authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                authClaims.Add(new Claim(ClaimTypes.Role, role));
                authClaims.Add(new Claim("userId", user.Id.ToString()));
                authClaims.Add(new Claim("fullName", user.FullName));
            }
            var accessToken = GenerateJsonWebToken.CreateToken(authClaims, configuration, DateTime.UtcNow);
            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        public static string GenerateRefreshToken(string email, IConfiguration configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
            };
            var refreshToken = GenerateJsonWebToken.CreateRefreshToken(claims, configuration, DateTime.UtcNow);
            return new JwtSecurityTokenHandler().WriteToken(refreshToken).ToString();
        }
    }
}
