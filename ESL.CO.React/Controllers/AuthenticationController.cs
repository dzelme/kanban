using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.LdapCredentialCheck;
using System.Net.Http;
using System.Net;

namespace ESL.CO.React.Controllers
{
    [Route("api/account")]
    public class AuthenticationController : Controller
    {
        private LdapClient ldap = new LdapClient();

        public AuthenticationController(LdapClient ldap)
        {
            this.ldap = ldap;
        }

        [HttpPost("[action]")]
        public Task<HttpResponseMessage> Login([FromBody]string username, [FromBody]string password)
        {
            HttpResponseMessage response = new HttpResponseMessage(); 
            if (ldap.CheckCredentials(username, password))
            {
                //return 200 + jwt
                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                //return 401
                response.StatusCode = HttpStatusCode.Unauthorized;
            }

            //response.Content = new StringContent(message);
            return Task.FromResult(response);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}