using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService (IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("cannot access token key from appsettings"); // tokenkey is null
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        //claims about the use
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserName)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds

        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }
}
