using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.LdapCredentialCheck;
using System.Net.Http;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ESL.CO.React.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ESL.CO.React.Controllers
{
    [Route("api/account/login")]
    public class AuthenticationController : Controller
    {
        //private LdapClient ldap = new LdapClient();

        //public AuthenticationController(LdapClient ldap)
        //{
        //    this.ldap = ldap;
        //}

        //[HttpPost]
        //public Task<HttpResponseMessage> Login([FromBody]string username, [FromBody]string password)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage();
        //    if (ldap.CheckCredentials(username, password))
        //    {
        //        //return 200 + jwt
        //        response.StatusCode = HttpStatusCode.OK;
        //    }
        //    else
        //    {
        //        //return 401
        //        response.StatusCode = HttpStatusCode.Unauthorized;
        //    }

        //    //response.Content = new StringContent(message);
        //    return Task.FromResult(response);
        //}


        [HttpPost]
        public IActionResult IssueJwtToken([FromBody] Credentials credentials)
        {
            if (credentials.Username == "adzelme" && credentials.Password == "0TESTtest")
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

            return Unauthorized(); //BadRequest instead
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}