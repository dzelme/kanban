﻿using System;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.LdapCredentialCheck;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ESL.CO.React.Models;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A controller for actions related to user accounts.
    /// </summary>
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILdapClient ldapClient;
        private readonly IOptions<JwtSettings> jwtSettings;

        public AccountController(ILdapClient ldapClient, IOptions<JwtSettings> jwtSettings)
        {
            this.ldapClient = ldapClient;
            this.jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Checks user credentials and the corresponding role before issuing a JWT.
        /// </summary>
        /// <param name="credentials">User's username and password combination to be checked.</param>
        /// <returns>
        /// A response with status code 401 if credentials were unrecognized and
        /// a response with status code 200 together with a JWT if credentials were valid and user belongs to the "admin" group.
        /// </returns>
        [HttpPost("[action]")]
        public IActionResult Login([FromBody] Credentials credentials)
        {
            if (credentials.Username == "" || credentials.Password == "") return Unauthorized();
            if (ldapClient.CheckCredentials(credentials.Username, credentials.Password))
            {
                var token = CreateToken(credentials);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Checks if the current user has a valid JWT (i.e., is authenticated).
        /// </summary>
        /// <returns>
        /// A response with status code 401 if the received JWT is invalid and
        /// a response with status code 200 if the JWT is valid.
        /// </returns>
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult HasValidJwt()
        {
            return Ok();
        }

        /// <summary>
        /// Creates a JWT based on user credentials, signing key and other information.
        /// </summary>
        /// <param name="credentials">The username and password of the user for whom the token will be created.</param>
        /// <returns>A JSON Web Token.</returns>
        private JwtSecurityToken CreateToken(Credentials credentials)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, credentials.Username),
                new Claim(ClaimTypes.Role, "Admins")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Value.Issuer,
                audience: jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return token;
        }
    }
}