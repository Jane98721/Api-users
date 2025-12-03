using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApiUsers.Entities;
using MyApiUsers.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace MyApiUsers.Controllers
{

[Route("api/[controller]")]
[ApiController]

public class AuthController: ControllerBase
{
  private readonly IConfiguration _configuration;
  public static List<User> users = new List <User>();

  public AuthController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  [HttpPost("register")]
  public ActionResult<User> Register(UserModels request)
  {
    var tempUser = new User();
    var hashedPassword = new PasswordHasher<User>()
    .HashPassword(tempUser, request.Password);

    var newUser = new User{
      Username = request.Username,
      PasswordHash = hashedPassword
    };

    users.Add(newUser);

    return Ok(newUser);

  }

  [HttpPost("login")]
  public ActionResult Login(UserModels request) 
  {
    var existingUser = users.FirstOrDefault(u => u.Username == request.Username);
    if(existingUser == null)
    {
      return BadRequest("User not found");
    }

    if(new PasswordHasher<User>()
    .VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.Password)
    == PasswordVerificationResult.Failed) 
    {
        return BadRequest("wrong password");
    }

    string token = CreateToken(existingUser);

    return Ok(new {token});

  }

  private string CreateToken(User user)
  {
    var claims = new List <Claim>
    {
      new Claim (ClaimTypes.Name, user.Username)
    };

    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

      var tokenDescriptor = new JwtSecurityToken(
        issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
        audience: _configuration.GetValue<string>("AppSettings:Audience"),
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials:creds
      );

      return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
  }
  }
}