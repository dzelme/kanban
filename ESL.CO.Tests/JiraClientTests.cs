﻿using Xunit;
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

namespace ESL.CO.Tests
{
    public class JiraClientTests
    {
        private Mock<IDbClient> dbClient;
        private Mock<JiraClient> jiraClient;
        private Mock<HttpMessageHandler> messageHandler;  //
        private Credentials credentials;
        private string presentationId;
        private BoardPresentationDbModel presentationDbModel;
        private BoardList firstPage, secondPage;

        private HttpClient client;

        public JiraClientTests()
        {
            dbClient = new Mock<IDbClient>();
            jiraClient = new Mock<JiraClient>(dbClient.Object) { CallBase = true };
            messageHandler = new Mock<HttpMessageHandler>();  //
            credentials = new Credentials { Username = "", Password = "" };
            presentationId = "1";



            var handler = new FakeHttpMessageHandler();
            client = new HttpClient(handler);



            presentationDbModel = new BoardPresentationDbModel
            {
                Id = presentationId,
                Credentials = credentials,
            };

            firstPage = new BoardList()
            {
                IsLast = false,
                Values = new List<Value>() {
                    new Value {  Id = "74" },
                    new Value {  Id = "75" },
                },
                StartAt = 0,
                MaxResults = 2
            };

            secondPage = new BoardList()
            {
                IsLast = true,
                Values = new List<Value>() {
                    new Value {  Id = "76" },
                    new Value {  Id = "77" },
                },
                StartAt = 2,
                MaxResults = 2
            };
        }

        //[Fact]  /////////////////////
        //public void GetBoardDataAsync_Should_Return_xxxxxxxx_If_Request_Successful()
        //{
        //    //// Arrange
        //    //const string testContent = "test content";
        //    ////messageHandler.Protected()
        //    ////    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //    ////    .Returns(Task.FromResult(new HttpResponseMessage
        //    ////    {
        //    ////        StatusCode = HttpStatusCode.OK,
        //    ////        Content = new StringContent(testContent)
        //    ////    }));
        //    ////var client = new HttpClient(messageHandler.Object);

        //    // does not work cause sendasync not virtual
        //    var ok = new HttpResponseMessage
        //    {
        //        StatusCode = HttpStatusCode.OK,
        //        Content = new StringContent(@"{""maxResults"":2,""startAt"":0,""isLast"":true,""values"":null}")
        //    };
        //    jiraClient.Setup(a => a.client.SendAsync(It.IsAny<HttpRequestMessage>())).Returns(Task.FromResult(ok)).Verifiable();

        //    // Act
        //    var actual = jiraClient.Object.GetBoardDataAsync<BoardList>("agile/1.0/board/", credentials, "", "").Result;

        //    // Assert
        //    jiraClient.Verify();
        //    Assert.NotNull(actual);
        //    Assert.True(actual.IsLast);
        //}


        [Fact]
        public void GetFullBoardList_Should_Return_Empty_List_If_Presentation_Id_Provided_And_No_BoardList_Recieved()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation(presentationId)).Returns(Task.FromResult(presentationDbModel)).Verifiable();
            jiraClient.Setup(s => s.GetBoardDataAsync<BoardList>("agile/1.0/board/", credentials, "", "")).Returns(Task.FromResult<BoardList>(null)).Verifiable();

            // Act
            var actual = jiraClient.Object.GetFullBoardList(null, presentationId).Result;

            // Assert
            dbClient.Verify();
            jiraClient.Verify();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetFullBoardList_Should_Return_Empty_List_If_Credentials_Provided_And_No_BoardList_Recieved()
        {
            // Arrange
            jiraClient.Setup(s => s.GetBoardDataAsync<BoardList>("agile/1.0/board/", credentials, "", "")).Returns(Task.FromResult<BoardList>(null)).Verifiable();

            // Act
            var actual = jiraClient.Object.GetFullBoardList(credentials, null).Result;

            // Assert
            jiraClient.Verify();
            Assert.False(actual.Any());
        }

        [Fact]
        public void GetFullBoardList_Should_Return_A_List_Of_Values_From_A_Single_Page()
        {
            // Arrange
            firstPage.IsLast = true;
            jiraClient
                .Setup(a => a.GetBoardDataAsync<BoardList>(It.IsAny<string>(), credentials, "", ""))
                .Returns((string a, Credentials b, string i, string j) =>
            {
                switch (a)
                {
                    case "agile/1.0/board/": return Task.FromResult(firstPage);
                    case "agile/1.0/board?startAt=2": return Task.FromResult<BoardList>(null);
                    default: return Task.FromResult<BoardList>(null);
                }
            }).Verifiable();

            // Act
            var actual = jiraClient.Object.GetFullBoardList(credentials, null).Result;

            // Assert
            jiraClient.Verify();
            Assert.Equal(2, actual.Count());
            Assert.Contains(actual, x => x.Id == "74");
            Assert.Contains(actual, x => x.Id == "75");
        }

        [Fact]
        public void GetFullBoardList_Should_Return_A_List_Of_Values_From_Multiple_Pages()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation(presentationId)).Returns(Task.FromResult(presentationDbModel)).Verifiable();
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>(It.IsAny<string>(), credentials, "", "")).Returns((string a, Credentials b, string i, string j) =>
            {
                switch (a)
                {
                    case "agile/1.0/board/": return Task.FromResult(firstPage);
                    case "agile/1.0/board?startAt=2": return Task.FromResult(secondPage);
                    default: return Task.FromResult<BoardList>(null);
                }
            }).Verifiable();

            // Act
            var actual = jiraClient.Object.GetFullBoardList(null, presentationId).Result;

            // Assert
            dbClient.Verify();
            jiraClient.Verify();
            Assert.Equal(4, actual.Count());
            Assert.Contains(actual, x => x.Id == "74");
            Assert.Contains(actual, x => x.Id == "75");
            Assert.Contains(actual, x => x.Id == "76");
            Assert.Contains(actual, x => x.Id == "77");
        }
    }
}

