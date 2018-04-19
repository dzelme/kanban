using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.DbConnection;
using System.Net;
using System.Net.Http;
using System.Linq;
using Moq;
using Moq.Protected;
using Novell.Directory.Ldap;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ESL.CO.React.LdapCredentialCheck;

namespace ESL.CO.Tests
{
    public class LdapClientTests
    {
        private NullLogger<LdapClient> logger;
        private IOptions<LdapSettings> ldapSettings;
        private LdapClient client;
        private Mock<LdapClient> ldapClient;

        private Mock<LdapConnection> ldapConnection;
        private LdapUser ldapUser;
        private string username, password;



        //private Mock<IDbClient> dbClient;
        //private Mock<JiraClient> jiraClient;
        //private Mock<HttpMessageHandler> messageHandler;  //
        //private Credentials credentials;
        //private string presentationId;
        //private BoardPresentationDbModel presentationDbModel;
        //private BoardList firstPage, secondPage;

        public LdapClientTests()
        {
            //dbClient = new Mock<IDbClient>();
            //jiraClient = new Mock<JiraClient>(dbClient.Object) { CallBase = true };
            //messageHandler = new Mock<HttpMessageHandler>();  //
            //credentials = new Credentials { Username = "", Password = "" };
            //presentationId = "1";

            ldapSettings = Options.Create(new LdapSettings
            {
                LdapServerUrl = "",
                DomainPrefix = "",
                SearchBase = "",
                SearchFilter = "",
                AdminCn = ""
            });
            logger = new NullLogger<LdapClient>();
            ldapClient = new Mock<LdapClient>(ldapSettings, logger);
            client = new LdapClient(ldapSettings, logger);

            ldapConnection = new Mock<LdapConnection>();
            ldapUser = new LdapUser
            {
                DisplayName = "ldap user",
                Username = "ldap_username"
            };
        }

        [Fact]
        public void CheckCredentials_Should_Return_False_If_Unable_To_Establish_A_Connection_With_LDAP_Server()
        {
            // Arrange

            ldapConnection.Setup(a => a.Connect(ldapSettings.Value.LdapServerUrl, 389));
            ldapConnection.Setup(s => s.Bind(ldapSettings.Value.DomainPrefix + username, password));
            //ldapClient.Setup(a => a.GetUserData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LdapConnection>())).Returns(ldapUser);

            // Act
            var actual = client.CheckCredentials("", "", false);

            // Assert
            Assert.Throws<LdapException>(() => actual);
            Assert.False(actual);
        }

        [Fact]
        public void CheckCredentials_Should_Return_False_If_After_Successful_Connection_Admin_Role_Required_And_User_Is_Not_Admin()
        {

        }

        [Fact]
        public void CheckCredentials_Should_Return_True_If_After_Successful_Connection_Admin_Role_Required_And_User_Is_Admin()
        {

        }

        [Fact]
        public void CheckCredentials_Should_Return_True_If_After_Successful_Connection_Admin_Role_Not_Required()
        {

        }
    }
}

