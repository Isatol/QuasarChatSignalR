using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private HACSYS.Security.JWT _jWT;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _chatHub;
        public AuthController(IConfiguration configuration, IHubContext<ChatHub> chatHub)
        {
            _jWT = new HACSYS.Security.JWT();
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
            string token = new JwtSecurityTokenHandler().WriteToken(await _jWT.CreateAsync("localhost", "localhost", claims, 10, "Localhost.2020*****"));
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
    }
}
