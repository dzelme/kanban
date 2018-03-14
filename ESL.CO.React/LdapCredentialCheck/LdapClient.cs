using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace ESL.CO.React.LdapCredentialCheck
{
    public class LdapClient : ILdapClient
    {
        public bool CheckCredentials(string username, string password)
        {
            // Creating an LdapConnection instance 
            using (var ldapConn = new LdapConnection())
            {
                //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                ldapConn.Connect("RIXMISDC04.internal.corp", 389);

                try
                {
                    //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
                    ldapConn.Bind(username, password);
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
