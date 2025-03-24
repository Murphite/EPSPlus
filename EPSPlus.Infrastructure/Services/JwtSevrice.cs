﻿

using EPSPlus.Domain.Entities;
using EPSPlus.Domain.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EPSPlus.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyString = _config.GetSection("JWT:Key").Value;

        if (string.IsNullOrEmpty(keyString))
        {
            throw new ArgumentNullException("JWT:Key", "The JWT key configuration is missing or empty.");
        }

        var key = Encoding.UTF8.GetBytes(keyString);

        // Ensure the key is at least 32 bytes long
        if (key.Length < 32)
        {
            throw new ArgumentOutOfRangeException("JWT:Key", "The key size must be at least 256 bits (32 bytes).");
        }

        var claimList = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(JwtRegisteredClaimNames.Sub, user.Id),
        new(JwtRegisteredClaimNames.Name, $"{user.FullName}"),
        new(JwtRegisteredClaimNames.Email, user.Email!)
    };
        claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _config.GetSection("JWT:Audience").Value,
            Issuer = _config.GetSection("JWT:Issuer").Value,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}

