using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CityGuide.Api.Data;
using CityGuide.Api.DTOs;
using CityGuide.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CityGuide.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepository;
        private IConfiguration _configuration;
        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDTO userForRegister)
        {
            if (await _authRepository.UserExists(userForRegister.UserName))
            {
                ModelState.AddModelError("UserName", "UserName already Exists");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userToCreate = new User()
            {
                UserName = userForRegister.UserName
            };
            var createdUser = await _authRepository.Register(userToCreate, userForRegister.Password);
            return StatusCode(201);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDTO userForLoginDTO)
        {
            var user = await _authRepository.Login(userForLoginDTO.UserName, userForLoginDTO.Password);
            if (user==null)
            {
                return Unauthorized();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName)
                }),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(tokenString);
        }
    }
}
