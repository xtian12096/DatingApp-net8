using System;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await UserExist(registerDto.Username)) return BadRequest("Username is Taken"); //use of UserExist function returning BadRequest response

        using var hmac = new HMACSHA512(); // declaring Sha512 HASh

        var user = new AppUser  //initialize this property
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), //implement password hashing
            PasswordSalt = hmac.Key

        };

        context.Users.Add(user);
        await context.SaveChangesAsync(); //LINQ Query of Adding inputted user account

        return new UserDto
        {
            Username = user.UserName,
            token = tokenService.CreateToken(user)
        };

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
        x.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        //decoding password salt and hash
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserDto
        {
            Username = user.UserName,
            token = tokenService.CreateToken(user)
        };
    }

    public async Task<bool> UserExist(string username) //Checking if Exist
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // Bob != bob
    }



}
