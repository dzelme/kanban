﻿using System;
using System.Linq;
using Novell.Directory.Ldap;
using ESL.CO.React.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ESL.CO.React.LdapCredentialCheck
{
    /// <summary>
    /// A class for connecting to LDAP server.
    /// </summary>
    public class LdapClient : ILdapClient
    {
        private readonly ILogger<LdapClient> logger;
        private readonly IOptions<LdapSettings> ldapSettings;
        private const string MemberOfAttribute = "memberOf";  // used in GetUserData
        private const string DisplayNameAttribute = "displayName";  // used in GetUserData
        private const string SAMAccountNameAttribute = "sAMAccountName";  // used in GetUserData

        public LdapClient(IOptions<LdapSettings> ldapSettings, ILogger<LdapClient> logger)
        {
            this.logger = logger;
            this.ldapSettings = ldapSettings;
        }

        /// <summary>
        /// Checks credentials against LDAP server.
        /// </summary>
        /// <param name="username">Username to be checked.</param>
        /// <param name="password">Password to be checked.</param>
        /// <param name="adminAccess">Specifies whether administrator access is required.</param>
        /// <returns>True if credentials recognized and false otherwise.</returns>
        public bool CheckCredentials(string username, string password, bool adminAccess = true)
        {
            // Creating an LdapConnection instance 
            using (var ldapConnection = new LdapConnection() { })
            {
                //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                ldapConnection.Connect(ldapSettings.Value.LdapServerUrl, 389);

                try
                {
                    //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
                    ldapConnection.Bind(ldapSettings.Value.DomainPrefix + username, password);
                }
                catch (LdapException e)
                {
                    logger.LogWarning("ldap exception on log in", e);
                    if (e.ResultCode == 49)  //LdapException: Invalid Credentials(49) Invalid Credentials
                    {
                        return false;
                    }
                    throw;
                }

                var ldapUser = GetUserData(username, password, ldapConnection);

                if (adminAccess) { return ldapUser.IsAdmin; }  // Returns true only if the user belongs to the group specified in appsettings.json as AdminCn in LdapSettings section
                else { return true; }
            }
        }

        /// <summary>
        /// Gets LDAP user data.
        /// </summary>
        /// <param name="username">LDAP username without domain prefix.</param>
        /// <param name="password">The password corresponding to the LDAP username.</param>
        /// <param name="ldapConnection">The active LDAP connection used for checking credential validity.</param>
        /// <returns>An object containing LDAP user's data including its username, display name and a boolean indicating membership to a specified group. </returns>
        private LdapUser GetUserData(string username, string password, LdapConnection ldapConnection)
        {
            var searchFilter = string.Format(ldapSettings.Value.SearchFilter, username);
            var result = ldapConnection.Search(
                ldapSettings.Value.SearchBase,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute },
                false
            );

            // Needed because of LDAP library imperfections (result.count sometimes won't register properly)
            var hasMore = result.hasMore();
            var count = result.Count;
            if (count != 1)
            {
                logger.LogWarning($"Unexpected response from LDAP server, found {count} users.");
                throw new ApplicationException($"Unexpected response from LDAP server, found {count} users.");
            }

            var entry = result.next();

            var user = new LdapUser
            {
                DisplayName = entry.getAttribute(DisplayNameAttribute).StringValue,
                Username = entry.getAttribute(SAMAccountNameAttribute).StringValue,
                IsAdmin = entry.getAttribute(MemberOfAttribute).StringValueArray.Contains(ldapSettings.Value.AdminCn)
            };
            
            return user;
        }
    }
}
