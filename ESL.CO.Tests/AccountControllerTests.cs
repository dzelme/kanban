using Xunit;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.LdapCredentialCheck;

namespace ESL.CO.Tests
{
    public class AccountControllerTests
    {
        private Mock<ILdapClient> ldapClient;
        private IOptions<JwtSettings> jwtSettings;
        private Credentials credentials;
        private AccountController controller;

        public AccountControllerTests()
        {
            ldapClient = new Mock<ILdapClient>();
            credentials = new Credentials { Username = "", Password = "" };

            jwtSettings = Options.Create(new JwtSettings
            {
                SigningKey = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                Issuer = "me",
                Audience = "you"
            });

            controller = new AccountController(ldapClient.Object, jwtSettings);
        }

        [Fact]
        public void Login_Should_Return_Unauthorized_If_Credentials_Empty()
        {
            // Arrange

            // Act
            var actual = controller.Login(credentials) as UnauthorizedResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(401, actual.StatusCode);
        }

        [Fact]
        public void Login_Should_Return_Unauthorized_If_Credentials_Invalid()
        {
            // Arrange
            credentials.Username = "username";
            credentials.Password = "password";
            ldapClient.Setup(a => a.CheckCredentials(credentials.Username, credentials.Password, true)).Returns(false).Verifiable();

            // Act
            var actual = controller.Login(credentials) as UnauthorizedResult;

            // Assert
            ldapClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(401, actual.StatusCode);
        }

        [Fact]
        public void Login_Should_Return_Ok_With_Jwt_Token_If_Credentials_Valid()
        {
            // Arrange
            credentials.Username = "username";
            credentials.Password = "password";
            ldapClient.Setup(a => a.CheckCredentials(credentials.Username, credentials.Password, true)).Returns(true).Verifiable();

            // Act
            var actual = controller.Login(credentials) as OkObjectResult;

            // Assert
            ldapClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
        }
    }
}

