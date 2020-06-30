using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {        
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _chatHub;
        public AuthController(IConfiguration configuration, IHubContext<ChatHub> chatHub)
        {            
            _configuration = configuration;
            _chatHub = chatHub;
        }

        [HttpPost, Route("[action]")]
        public async Task<IActionResult> Login([FromBody] Models.Auth auth)
        {
            var jti = Guid.NewGuid().ToString();
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("nombre", auth.Nombre));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti));
            string token = new JwtSecurityTokenHandler().WriteToken(await CreateToken("localhost", "localhost", claims, 10, "Localhost.2020*****"));
            return StatusCode(200, new 
            {
                token
            });
        }  
        
        [HttpGet, Route("[action]")]
        public async Task<IActionResult> AddToGroup(string connID, string grupo)
        {            
            await _chatHub.Groups.AddToGroupAsync(connID, grupo);            
            return StatusCode(200, new { 
                message = "Operation completed successfully"
            });
        }

        public async Task<JwtSecurityToken> CreateToken(string issuer, string audience, List<Claim> claims, double expirationMinutes, string securityKey)
        {
            return await Task.Run(() => new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                notBefore: DateTime.Now,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256)
                ));
        }
    }
}
