using System;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.LdapCredentialCheck;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ESL.CO.React.Models;
using System.Security.Claims;

namespace ESL.CO.React.Controllers
{
    [Route("api/account/login")]
    public class AuthenticationController : Controller
    {
        private readonly ILdapClient ldapClient;

        public AuthenticationController(ILdapClient ldapClient)
        {
            this.ldapClient = ldapClient;
        }

        [HttpPost]
        public IActionResult IssueJwtToken([FromBody] Credentials credentials)
        {
            if (ldapClient.CheckCredentials(credentials.Username, credentials.Password))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, credentials.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bGS6lzFqvvSQ8ALbOxatm7/Vk7mLQyzqaS34Q4oR1ew="));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "rixinterns01.internal.corp/kanban", //http://localhost:58533/
                    audience: "rixinterns01.internal.corp/kanban",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return Unauthorized();
        }
    }
}