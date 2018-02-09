using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using System.Text.Encodings;
using System.Net.Http.Headers;

//using ESL.CO.Helpers;

namespace ESL.CO.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult OneBoard()
        {
            int boardId = 961;  //620, 880, 961, 963
            //Board kp = new Board(boardId);
            //kp.ReadBoard();
            //kp.UpdateBoard();

            Helpers.JsonBoard kp = new Helpers.JsonBoard(boardId);
            var asd = 0;
            if (kp.UpdateBoard()) { ViewData["Message"] = "Board #" + boardId.ToString(); };


            //ViewData["Message"] = "Board #" + boardId.ToString();

            //finds total # of rows in table
            int rowCount = kp.Columns[0].Issues.Count();  //max no of rows
            for (int k = 1; k < kp.ColumnCount; k++)
            {
                if (kp.Columns[k].Issues.Count() > rowCount) { rowCount = kp.Columns[k].Issues.Count(); }
            }

            //finds each column's length, needed for drawing the table
            int[] columnLength = new int[kp.ColumnCount];
            for (int k = 0; k < kp.ColumnCount; k++)
            {
                columnLength[k] = kp.Columns[k].Issues.Count();
            }

            ViewBag.columnLength = columnLength;
            ViewBag.columnCount = kp.ColumnCount;
            ViewBag.rowCount = rowCount;
            ViewBag.columns = kp.Columns;

            return View();
        }

        public IActionResult AllBoards()
        {
            ViewData["Message"] = "Nolasītā informācija:";

            string BoardId;

            string credentials = "arumka:Dzukste22";


            //*************************************************************************************************************
            //Visi dēļi

            HttpClient client = new HttpClient();           
              
             client.BaseAddress = new Uri("http://localhost:50973/");
             client.DefaultRequestHeaders.Accept.Clear();
             client.DefaultRequestHeaders.Accept.Add(           
                    new MediaTypeWithQualityHeaderValue("application/json"));


            
            async Task<BoardList> GetIssuesAsync()
            {

                client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

                var response = await client.GetAsync("https://jira.returnonintelligence.com/rest/agile/1.0/board");
                
                if (response.IsSuccessStatusCode)
                {
                    var serializer = new Newtonsoft.Json.JsonSerializer();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        ViewData["xxx"] = serializer.Deserialize<BoardList>(jsonReader);

                        return serializer.Deserialize<BoardList>(jsonReader);
                    }
                }

                throw new InvalidOperationException();
            }

            



/*

            string urlAllBoards = "https://jira.returnonintelligence.com/rest/agile/1.0/board";

            WebRequest myReq = WebRequest.Create(urlAllBoards);
            myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            WebResponse wr = myReq.GetResponse();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            JObject j = JObject.Parse(content);

            BoardList BL = new BoardList
            {
                IsLast = (string)j.SelectToken("isLast")
            };

            var AllBoardObjects = j.SelectToken("values");
            List<Board> CurrentBoardList = new List<Board>();

            foreach (JObject board in AllBoardObjects)
            {
                Board B = new Board
                {
                    ID = (string)board.SelectToken("id"),
                    Name = (string)board.SelectToken("name"),
                    Type = (string)board.SelectToken("type"),
                    Link = (string)board.SelectToken("self")
                };

                CurrentBoardList.Add(B);
            }

            BL.AllBoards = CurrentBoardList;
*/

            //*************************************************************************************************************

            //*******************************************************************************************************************
            //Dēļa konfigurācija          
            /*
            string urlConfig;

            foreach (var board in BL.AllBoards)
            {

                BoardId = board.ID;
                urlConfig = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/configuration";

                myReq = WebRequest.Create(urlConfig);
                myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                wr = myReq.GetResponse();
                receiveStream = wr.GetResponseStream();
                reader = new StreamReader(receiveStream, Encoding.UTF8);
                content = reader.ReadToEnd();
                j = JObject.Parse(content);


                ColumnList CL = new ColumnList
                {
                    BoardId = (string)j.SelectToken("id"),
                    BoardName = (string)j.SelectToken("name"),
                    BoardType = (string)j.SelectToken("type")
                };


                var AllColumnObjects = j.SelectToken("columnConfig.columns");
                List<Column> CurrentColumnList = new List<Column>();

                foreach (JObject column in AllColumnObjects)
                {
                    Column C = new Column
                    {
                        Name = (string)column.SelectToken("name"),
                       // Max = (int)column.SelectToken("max")
                    };

                    CurrentColumnList.Add(C);

                }

                CL.AllColumns = CurrentColumnList;
                board.BoardColumns = CL;                

            }
            //*****************************************************************************************************************


            //****************************************************************************************************************
            //Dēļa ieraksti

            string urlIssue;

            foreach (var board in BL.AllBoards)
            {

                BoardId = board.ID;
                urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue";

                myReq = WebRequest.Create(urlIssue);
                myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                wr = myReq.GetResponse();
                receiveStream = wr.GetResponseStream();
                reader = new StreamReader(receiveStream, Encoding.UTF8);
                content = reader.ReadToEnd();
                j = JObject.Parse(content);

              

                var AllIssueObjects = j.SelectToken("issues");
                List<Issue> CurrentIssueList = new List<Issue>();           

                foreach (var issue in AllIssueObjects)
                {
                    Issue I = new Issue
                    {
                        ID = (string)issue.SelectToken("id"),
                        Key = (string)issue.SelectToken("key"),
                        Priority = (string)issue.SelectToken("fields.priority.name"),
                        Assignee = (string)issue.SelectToken("fields.assignee.name"),
                        Summary = (string)issue.SelectToken("fields.summary"),
                        Link = (string)issue.SelectToken("self")
                    };

                    I.Status = (string)issue.SelectToken("fields.status.statusCategory.name");

                    if (I.Status == "No Category")
                    {
                        I.Status = (string)issue.SelectToken("fields.status.name");
                    }

                    CurrentIssueList.Add(I);
                }

                int TotalIssueCount = (int)j.SelectToken("total");
                int StartAtCount = (int)j.SelectToken("startAt");
                
                if (TotalIssueCount > 50)
                {
                    while (TotalIssueCount > StartAtCount + 50)
                    {
                        StartAtCount += 50;
                        urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue?startAt=" + StartAtCount;


                        myReq = WebRequest.Create(urlIssue);
                        myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                        wr = myReq.GetResponse();
                        receiveStream = wr.GetResponseStream();
                        reader = new StreamReader(receiveStream, Encoding.UTF8);
                        content = reader.ReadToEnd();
                        j = JObject.Parse(content);


                        AllIssueObjects = j.SelectToken("issues");

                        foreach (var issue in AllIssueObjects)
                        {
                            Issue I = new Issue
                            {
                                ID = (string)issue.SelectToken("id"),
                                Priority = (string)issue.SelectToken("fields.priority.name"),
                                Assignee = (string)issue.SelectToken("fields.assignee.name"),
                                Summary = (string)issue.SelectToken("fields.summary"),
                                Link = (string)issue.SelectToken("self")
                            };

                            I.Status = (string)issue.SelectToken("fields.status.statusCategory.name");

                            if (I.Status == "No Category")
                            {
                                I.Status = (string)issue.SelectToken("fields.status.name");
                            }


                            CurrentIssueList.Add(I);
                        }
                    }
                }
                
                string issueColumn;

                string ExpandValue = (string)j.SelectToken("expand");
                int TotalValue = (int)j.SelectToken("total");


                for (int i = 0; i < board.BoardColumns.AllColumns.Count; i++)
                {
                    IssueList IL = new IssueList
                    {
                        Expand = ExpandValue,
                        Total = TotalValue,
                    };

                    issueColumn = board.BoardColumns.AllColumns[i].Name;

                    List<Issue> tmp = new List<Issue>();

                    foreach (var item in CurrentIssueList)
                    {
                        if (item.Status == issueColumn)
                        {
                            tmp.Add(item);
                        }
                    }

                    IL.AllIssues = tmp;
                    board.BoardColumns.AllColumns[i].ColumnIssues = IL;
                 
                }


                int maxIssues = 0;
                for (int i = 0; i < board.BoardColumns.AllColumns.Count; i++)
                {
                    if (board.BoardColumns.AllColumns[i].ColumnIssues.AllIssues.Count > maxIssues)
                    {
                        maxIssues = board.BoardColumns.AllColumns[i].ColumnIssues.AllIssues.Count;
                    }
                }


                board.MaxIssueCount = maxIssues;

            }



            var asd = "";


            foreach (var item in BL.AllBoards)
            {
                asd += item.ID + " | " + item.BoardColumns.AllColumns.Count + " | " + item.MaxIssueCount + " | ";

                foreach (var n in item.BoardColumns.AllColumns)
                {
                    asd += n.ColumnIssues.AllIssues.Count + Environment.NewLine;
                }
                asd += "||";
            }

            ViewData["js"] = asd;

            //*************************************************************************************************************

            ViewBag.BoardList = BL.AllBoards;
            */

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
