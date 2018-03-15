using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;
using ESL.CO.React.Models;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.LdapCredentialCheck
{
    /// <summary>
    /// A class for connecting to LDAP server.
    /// </summary>
    public class LdapClient : ILdapClient
    {
        private readonly IOptions<LdapSettings> ldapSettings;

        public LdapClient (IOptions<LdapSettings> ldapSettings)
        {
            this.ldapSettings = ldapSettings;
        }

        /// <summary>
        /// Checks credentials against LDAP server.
        /// </summary>
        /// <param name="username">Username to be checked.</param>
        /// <param name="password">Password to be checked.</param>
        /// <returns>True if credentials recognized and false otherwise.</returns>
        public bool CheckCredentials(string username, string password)
        {
            // Creating an LdapConnection instance 
            using (var ldapConn = new LdapConnection() { })
            {
                //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                ldapConn.Connect(ldapSettings.Value.LdapServerUrl, 389);

                try
                {
                    //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
                    ldapConn.Bind(ldapSettings.Value.DomainPrefix + username, password);
                }
                catch (LdapException e)
                {
                    if (e.ResultCode == 49)  //LdapException: Invalid Credentials(49) Invalid Credentials
                    {
                        return false;
                    }
                    throw;
                }

                return true;
            }
        }
    }
}
