using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.DbConnection;
using System.Collections.Generic;
using System.Linq;
using Moq;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.LdapCredentialCheck;

namespace ESL.CO.Tests
{
    public class PresentationsControllerTests
    {
        private Mock<IJiraClient> jiraClient;
        private Mock<IDbClient> dbClient;
        private Mock<ILdapClient> ldapClient;
        private PresentationsController controller;
        private BoardPresentationDbModel presentation1, presentation2;
        private BoardPresentation boardPresentation;
        private IEnumerable<BoardPresentationDbModel> presentationList;
        private Credentials credentials;
        private BoardName boardName1, boardName2;

        public PresentationsControllerTests()
        {
            jiraClient = new Mock<IJiraClient>();
            dbClient = new Mock<IDbClient>();
            ldapClient = new Mock<ILdapClient>();
            credentials = new Credentials { Username = "", Password = "" };

            presentation1 = new BoardPresentationDbModel()
            {
                Id = "1",
                Title = "first presentation",
                Owner = "first owner",
                Credentials = new Credentials
                {
                    Username = "first username",
                    Password = "123"
                },
                Boards = new List<BoardDbModel>()
                {
                    new BoardDbModel() { Id = "74" },
                    new BoardDbModel() { Id = "75" }
                }
            };

            presentation2 = new BoardPresentationDbModel()
            {
                Id = "2",
                Title = "second presentation",
                Owner = "second owner",
                Credentials = new Credentials
                {
                    Username = "second username",
                    Password = "098"
                },
                Boards = new List<BoardDbModel>()
                {
                    new BoardDbModel() { Id = "76" },
                    new BoardDbModel() { Id = "77" }
                }
            };

            boardPresentation = new BoardPresentation
            {
                Id = "3",
                Title = "third presentation",
                Owner = "third owner",
                Credentials = new Credentials
                {
                    Username = "third username",
                    Password = "4576"
                },
                Boards = new FullBoardList
                {
                    Values = new List<Value>()
                    {
                        new Value() { Id = "74" },
                        new Value() { Id = "75" }
                    }
                }
            };

            presentationList = new List<BoardPresentationDbModel>()
            {
                presentation1,
                presentation2
            };

            boardName1 = new BoardName
            {
                Id = "74",
                Name = "board1 name"
            };

            boardName2 = new BoardName
            {
                Id = "75",
                Name = "board2 name"
            };

            controller = new PresentationsController(jiraClient.Object, dbClient.Object, ldapClient.Object);
        }

        [Fact]
        public void GetPresentations_Should_Return_Ok_And_Presentation_List_With_Board_Names_From_Database()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentationsListAsync()).Returns(Task.FromResult(presentationList)).Verifiable();

            // Act
            var actual = controller.GetPresentations().Result as OkObjectResult;

            // Assert
            dbClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.Equal(2, (actual.Value as List<BoardPresentation>).Count());
            Assert.NotNull((actual.Value as List<BoardPresentation>).ElementAt(0).Boards.Values.ElementAt(0).Name);
            Assert.NotNull((actual.Value as List<BoardPresentation>).ElementAt(0).Boards.Values.ElementAt(1).Name);
            Assert.NotNull((actual.Value as List<BoardPresentation>).ElementAt(1).Boards.Values.ElementAt(0).Name);
            Assert.NotNull((actual.Value as List<BoardPresentation>).ElementAt(1).Boards.Values.ElementAt(1).Name);
            Assert.Null((actual.Value as List<BoardPresentation>).ElementAt(0).Credentials);
            Assert.Null((actual.Value as List<BoardPresentation>).ElementAt(1).Credentials);
        }

        [Fact]
        public void GetPresentations_Should_Return_Ok_And_Empty_Presentation_List()
        {
            // Arrange
            var emptyList = new List<BoardPresentationDbModel>();
            IEnumerable<BoardPresentationDbModel> empty = emptyList;
            dbClient.Setup(a => a.GetPresentationsListAsync()).Returns(Task.FromResult(empty)).Verifiable();

            // Act
            var actual = controller.GetPresentations().Result as OkObjectResult;

            // Assert
            dbClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.Empty((actual.Value as List<BoardPresentation>));
        }



        [Fact]
        public void GetPresentation_Should_Return_BadRequest_If_No_Presentation_With_The_Specified_Id_Exists()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation("x")).Returns(Task.FromResult<BoardPresentationDbModel>(null)).Verifiable();

            // Act
            var actual = controller.GetPresentation("x").Result as BadRequestObjectResult;

            // Assert
            dbClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);

        }

        [Fact]
        public void GetPresentation_Should_Return_Ok_And_A_Single_Presentation_With_Board_Names_From_Database_And_No_Password()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation("1")).Returns(Task.FromResult(presentation1)).Verifiable();

            // Act
            var actual = controller.GetPresentation("1").Result as OkObjectResult;

            // Assert
            dbClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.NotNull((actual.Value as BoardPresentation).Boards.Values.ElementAt(0).Name);
            Assert.NotNull((actual.Value as BoardPresentation).Boards.Values.ElementAt(1).Name);
            Assert.Null((actual.Value as BoardPresentation).Credentials.Password);
        }

        [Fact]
        public void MakeViewable_Should_Return_A_Presentation_With_Actual_Board_Names()
        {
            // Arrange
            jiraClient.Setup(s => s.GetBoardDataAsync<BoardName>(It.IsAny<string>(), It.IsAny<Credentials>(), "", "")).Returns((string a, Credentials b, string i, string j) =>
            {
                switch (a)
                {
                    case "agile/1.0/board/74": return Task.FromResult(boardName1);
                    case "agile/1.0/board/75": return Task.FromResult(boardName2);
                    default: return Task.FromResult<BoardName>(null);
                }
            }).Verifiable();

            // Act
            var actual = controller.MakeViewable(presentation1).Result;

            // Assert
            jiraClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal("board1 name", actual.Boards.Values.ElementAt(0).Name);
            Assert.Equal("board2 name", actual.Boards.Values.ElementAt(1).Name);
        }

        [Fact]
        public void MakeViewable_Should_Return_A_Presentation_With_Default_Board_Names()
        {
            // Arrange
            jiraClient.Setup(s => s.GetBoardDataAsync<BoardName>(It.IsAny<string>(), It.IsAny<Credentials>(), "", "")).Returns(Task.FromResult<BoardName>(null)).Verifiable();

            // Act
            var actual = controller.MakeViewable(presentation1).Result;

            // Assert
            jiraClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal("<panelis nepieejams>", actual.Boards.Values.ElementAt(0).Name);
            Assert.Equal("<panelis nepieejams>", actual.Boards.Values.ElementAt(1).Name);
        }

        [Fact]
        public void SavePresentation_Should_Return_Ok_And_A_Presentation_Without_Credentials()
        {
            // Arrange
            ldapClient.Setup(s => s.CheckCredentials(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(true).Verifiable();

            // Act
            var actual = controller.SavePresentation(boardPresentation).Result as OkObjectResult;

            // Assert
            ldapClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);

            Assert.Equal("3", (actual.Value as BoardPresentation).Id);
            Assert.Equal("third presentation", (actual.Value as BoardPresentation).Title);
            Assert.Equal("third owner", (actual.Value as BoardPresentation).Owner);
            Assert.Null((actual.Value as BoardPresentation).Credentials);
            Assert.Equal("74", (actual.Value as BoardPresentation).Boards.Values.ElementAt(0).Id);
            Assert.Equal("75", (actual.Value as BoardPresentation).Boards.Values.ElementAt(1).Id);
        }

        [Fact]
        public void SavePresentation_Should_Return_Ok_And_A_Presentation_Without_Credentials_If_No_Password_Supplied()
        {
            // Arrange
            var boardPresentationDbModel = new BoardPresentationDbModel
            {
                Credentials = boardPresentation.Credentials
            };
            boardPresentation.Credentials.Password = null;
            dbClient.Setup(a => a.GetPresentation(It.IsAny<string>())).Returns(Task.FromResult(boardPresentationDbModel)).Verifiable();
            ldapClient.Setup(s => s.CheckCredentials(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(true).Verifiable();

            // Act
            var actual = controller.SavePresentation(boardPresentation).Result as OkObjectResult;

            // Assert
            dbClient.Verify();
            ldapClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);

            Assert.Equal("3", (actual.Value as BoardPresentation).Id);
            Assert.Equal("third presentation", (actual.Value as BoardPresentation).Title);
            Assert.Equal("third owner", (actual.Value as BoardPresentation).Owner);
            Assert.Null((actual.Value as BoardPresentation).Credentials);
            Assert.Equal("74", (actual.Value as BoardPresentation).Boards.Values.ElementAt(0).Id);
            Assert.Equal("75", (actual.Value as BoardPresentation).Boards.Values.ElementAt(1).Id);
        }

        [Fact]
        public void SavePresentation_Should_Return_BadRequest_If_Invalid_Model_State()
        {
            // Arrange
            ldapClient.Setup(s => s.CheckCredentials(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(true).Verifiable();
            controller.ModelState.AddModelError("", "Error");

            // Act
            var actual = controller.SavePresentation(boardPresentation).Result as BadRequestObjectResult;

            // Assert
            ldapClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
        }

        [Fact]
        public void SavePresentation_Should_Return_Unauthorized_If_Erroneous_Credentials_Supplied()
        {
            // Arrange
            ldapClient.Setup(s => s.CheckCredentials(It.IsAny<string>(), It.IsAny<string>(), false)).Returns(false).Verifiable();

            // Act
            var actual = controller.SavePresentation(boardPresentation).Result as UnauthorizedResult;

            // Assert
            ldapClient.Verify();
            Assert.NotNull(actual);
            Assert.Equal(401, actual.StatusCode);
        }
    }
}

