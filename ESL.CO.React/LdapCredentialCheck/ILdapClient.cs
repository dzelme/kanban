using ESL.CO.React.Models;

namespace ESL.CO.React.LdapCredentialCheck
{
    public interface ILdapClient
    {
        bool CheckCredentials(string username, string password);
        AppUser Login(string username, string password);
    }
}