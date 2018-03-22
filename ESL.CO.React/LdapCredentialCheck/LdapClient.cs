using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;
using ESL.CO.React.Models;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

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




        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";

        public AppUser Login(string username, string password)
        {
            var ldapConnection = new LdapConnection(); //
            ldapConnection.Connect(ldapSettings.Value.LdapServerUrl, 389); //
            //ldapConnection.Bind(ldapSettings.Value.DomainPrefix + ldapSettings.Value.DefaultUsername, ldapSettings.Value.DefaultPassword);
            ldapConnection.Bind(ldapSettings.Value.DomainPrefix + username, password);

            var searchFilter = string.Format(ldapSettings.Value.SearchFilter, username);
            var result = ldapConnection.Search(
                ldapSettings.Value.SearchBase,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute },
                false
            );

            try
            {
                var user = result.next();
                if (user != null)
                {
                    ldapConnection.Bind(user.DN, password);
                    if (ldapConnection.Bound)
                    {
                        var a = user.getAttribute(MemberOfAttribute).StringValueArray;
                        return new AppUser
                        {
                            DisplayName = user.getAttribute(DisplayNameAttribute).StringValue,
                            Username = user.getAttribute(SAMAccountNameAttribute).StringValue,
                            IsAdmin = user.getAttribute(MemberOfAttribute).StringValueArray.Contains(ldapSettings.Value.AdminCn)
                        };
                    }
                }
            }
            catch
            {
                throw new Exception("Login failed.");
            }
            ldapConnection.Disconnect();
            return null;
        }


        //private readonly LdapConnection _ldapConnection;

        // ...

        //public IEnumerable<string> GetGroupsForUser(string username)
        //{
        //    var ldapConnection = new LdapConnection(); //
        //    ldapConnection.Connect(ldapSettings.Value.LdapServerUrl, 389); //
        //    ldapConnection.Bind(ldapSettings.Value.DomainPrefix + ldapSettings.Value.DefaultUsername, ldapSettings.Value.DefaultPassword);

        //    var groups = new Stack<string>();
        //    var uniqueGroups = new HashSet<string>();

        //    foreach (string group in this.GetGroupsForUserCore(ldapSettings.Value.DomainPrefix + username, ldapConnection))
        //        groups.Push(group);

        //    while (groups.Count > 0)
        //    {
        //        string group = groups.Pop();
        //        if (uniqueGroups.Add(group))
        //            yield return group;

        //        foreach (string parentGroup in this.GetGroupsForUserCore(group, ldapConnection))
        //            groups.Push(parentGroup);
        //    }
        //}

        //private IEnumerable<string> GetGroupsForUserCore(string user, LdapConnection ldapConnection)
        //{
        //    LdapSearchQueue searchQueue = ldapConnection.Search(
        //        //_config["searchBase"],
        //        ldapSettings.Value.SearchBase,
        //        LdapConnection.SCOPE_SUB,
        //        $"(sAMAccountName={user})",
        //        new string[] { "cn", "memberOf" },
        //        false,
        //        null as LdapSearchQueue);

        //    LdapMessage message;
        //    while ((message = searchQueue.getResponse()) != null)
        //    {
        //        if (message is LdapSearchResult searchResult)
        //        {
        //            LdapEntry entry = searchResult.Entry;
        //            foreach (string value in HandleEntry(entry))
        //                yield return value;
        //        }
        //        else
        //            continue;
        //    }

        //    IEnumerable<string> HandleEntry(LdapEntry entry)
        //    {
        //        LdapAttribute attr = entry.getAttribute("memberOf");

        //        if (attr == null) yield break;

        //        foreach (string value in attr.StringValueArray)
        //        {
        //            string groupName = GetGroup(value);
        //            yield return groupName;
        //        }
        //    }

        //    string GetGroup(string value)
        //    {
        //        Match match = Regex.Match(value, "^CN=([^,]*)");

        //        if (!match.Success) return null;

        //        return match.Groups[1].Value;
        //    }
        //}
    }
}
