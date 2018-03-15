namespace ESL.CO.React.LdapCredentialCheck
{
    public interface ILdapClient
    {
        bool CheckCredentials(string username, string password);
    }
}