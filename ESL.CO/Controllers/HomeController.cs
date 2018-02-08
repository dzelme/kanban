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
//using ESL.CO.Helpers;

namespace ESL.CO.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index2()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";


            //string url2 = "http://localhost:50973/rest/api/2/issue";
            //string url = "https://jira.returnonintelligence.com/rest/api/2/issue/1111407";
            //https://jira.returnonintelligence.com/rest/api/2/search?jql=project="KP" & startAt="start" & maxResults="max"
            //string url = "https://jira.returnonintelligence.com/rest/api/2/search?jql=project=\"KP\" & startAt=\"start\" & maxResults=\"max\"";

            string board = "620";
            string urlConfig = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + board + "/configuration";

            WebRequest myReq = WebRequest.Create(urlConfig);
            #region
            string credentials = "user:pass!";
            #endregion
            //CredentialCache mycache = new CredentialCache();
            myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            WebResponse wr = myReq.GetResponse();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            JObject j = JObject.Parse(content);

            var efg = "";
            var columnArray = j.SelectToken("columnConfig.columns");
            var columnCount = columnArray.Count();  //4
            string[] columns = new string[columnCount];
            int counter = 0;
            foreach (JObject column in columnArray)
            {
                string columnName = (string)column.GetValue("name");  //returns column names
                columns[counter] = columnName;
                counter++;
                //efg += columnName;
            }

            for (int i = 0; i < columnCount; i++)
            {
                efg += columns[i];
            }

            ViewData["js2"] = efg;



                //board = "961";
                string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + board + "/issue?startAt=0";

                myReq = WebRequest.Create(urlIssue);
                myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                wr = myReq.GetResponse();
                receiveStream = wr.GetResponseStream();
                reader = new StreamReader(receiveStream, Encoding.UTF8);
                content = reader.ReadToEnd();
                j = JObject.Parse(content);

                int issueCount = (int)j.SelectToken("total"); //if larger than 50, multiple takes to read everything "startAt = 50"

                //var asd = "";

                List<Issue>[] kanbanBoard = new List<Issue>[columnCount];
                for (int k = 0; k < columnCount; k++)
                {
                    List<Issue> iList = new List<Issue>();
                    kanbanBoard[k] = iList;
                }

                for (int i = 0; i < 50; i++)  //(i < issueCount) - all issues, but max only 50 per request
                {
                    string id = (string)j.SelectToken("issues[" + i.ToString() + "].id");
                    string status = (string)j.SelectToken("issues[" + i.ToString() + "].fields.status.name");//statusCategory.name");
                    string priority = (string)j.SelectToken("issues[" + i.ToString() + "].fields.priority.name");
                    string assignee = (string)j.SelectToken("issues[" + i.ToString() + "].fields.assignee.name");
                    string summary = (string)j.SelectToken("issues[" + i.ToString() + "].fields.summary");
                    string link = "https://jira.returnonintelligence.com/browse/" + (string)j.SelectToken("issues[" + i.ToString() + "].key");

                    //required because status name that corresponds to column name can be in 2 different locations
                    string status2 = (string)j.SelectToken("issues[" + i.ToString() + "].fields.status.statusCategory.name");

                    int t = 1;
                    int appropriateColumn = -1;
                    for (int ii = 0; ii < columnCount; ii++)
                    {
                        if (String.Equals(status, columns[ii], StringComparison.OrdinalIgnoreCase)) { t = 1; appropriateColumn = ii; } 
                        if (String.Equals(status2, columns[ii], StringComparison.OrdinalIgnoreCase)) { t = 2;  appropriateColumn = ii; }  //
                    }
                    if (t == 2) { status = status2; } //resolves status name location ambiguity


                    /*

                    //creates object adds to table
                    Issue temp = new Issue(id, status, priority, assignee, summary, link);
                    //if (appropriateColumn != -1) { kanbanBoard[appropriateColumn].Add(temp); }
                    kanbanBoard[appropriateColumn].Add(temp);


                    */
                }

            /*
            var asd = "";
            foreach (var item in kanbanBoard[2])
            {
                asd += " " + item.summary;
            }
            ViewData["js"] = asd;
            */

            //finds total # of rows in table
            int rowCount = kanbanBoard[0].Count();  //max no of rows
            for (int k = 1; k < columnCount; k++)
            {
                if (kanbanBoard[k].Count() > rowCount) { rowCount = kanbanBoard[k].Count(); }
            }

            //finds each column's length, needed for drawing the table
            int[] columnLength = new int[columnCount];
            for (int k = 0; k < columnCount; k++)
            {
                columnLength[k] = kanbanBoard[k].Count();
            }

            ViewBag.columnLength = columnLength;
            ViewBag.columnCount = columnCount;
            ViewBag.rowCount = rowCount;
            ViewBag.columns = columns;
            ViewBag.kanbanBoard = kanbanBoard;

            return View();
        }

        public IActionResult Contact()
        {
            /*
            int boardId = 961;  //620, 880, 961, 963
            //Board kp = new Board(boardId);
            //kp.ReadBoard();
            //kp.UpdateBoard();

            JsonBoard kp = new JsonBoard(boardId);
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

            */
            return View();
        }

        public IActionResult Index()
        {
            ViewData["Message"] = "Nolasītā informācija:";

            string BoardId;

            string credentials = "user:pass";


            //*************************************************************************************************************
            //Visi dēļi

            string urlAllBoards = "https://jira.returnonintelligence.com/rest/agile/1.0/board";

            WebRequest myReq = WebRequest.Create(urlAllBoards);
            myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            WebResponse wr = myReq.GetResponse();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            JObject j = JObject.Parse(content);


            var AllBoards = j.SelectToken("values");
            int BoardCount = AllBoards.Count();

            List<Board> BoardList = new List<Board>();

            foreach (JObject board in AllBoards)
            {
                Board B = new Board
                {
                    ID = (string)board.SelectToken("id"),
                    Name = (string)board.SelectToken("name"),
                    Type = (string)board.SelectToken("type"),
                    Link = (string)board.SelectToken("self")
                };

                BoardList.Add(B);
            }


            //*************************************************************************************************************

            //*******************************************************************************************************************
            //Dēļa konfigurācija          

            string urlConfig;

            foreach (var board in BoardList)
            {
                List<Column> ColumnList = new List<Column>();

                BoardId = board.ID;
                urlConfig = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/configuration";

                myReq = WebRequest.Create(urlConfig);
                myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                wr = myReq.GetResponse();
                receiveStream = wr.GetResponseStream();
                reader = new StreamReader(receiveStream, Encoding.UTF8);
                content = reader.ReadToEnd();
                j = JObject.Parse(content);

                var AllColumns = j.SelectToken("columnConfig.columns");

                foreach (JObject column in AllColumns)
                {
                    Column C = new Column
                    {
                        Title = (string)column.SelectToken("name"),
                        ParentBoardId = BoardId
                    };

                    ColumnList.Add(C);

                }

                board.BoardColumns = ColumnList;
                board.ColumnCount = ColumnList.Count();

            }
            //*****************************************************************************************************************


            //****************************************************************************************************************
            //Dēļa ieraksti

            string urlIssue;
            string issueColumn;

            foreach (var board in BoardList)
            {
                BoardId = board.ID;

                List<Issue> IssueList = new List<Issue>();

                urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue";

                myReq = WebRequest.Create(urlIssue);
                myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                wr = myReq.GetResponse();
                receiveStream = wr.GetResponseStream();
                reader = new StreamReader(receiveStream, Encoding.UTF8);
                content = reader.ReadToEnd();
                j = JObject.Parse(content);


                var AllIssues = j.SelectToken("issues");

                foreach (var issue in AllIssues)
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

                    IssueList.Add(I);
                }

                int TotalIssueCount = (int)j.SelectToken("total"); //if larger than 50, multiple takes to read everything "startAt = 50"

                if (TotalIssueCount > 50)
                {
                    while (TotalIssueCount > 50)
                    {
                        urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue";

                        AllIssues = j.SelectToken("issues");

                        foreach (var issue in AllIssues)
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


                            IssueList.Add(I);
                        }

                        TotalIssueCount -= 50;
                    }
                }


                for (int i = 0; i < board.BoardColumns.Count; i++)
                {

                    issueColumn = board.BoardColumns[i].Title;

                    List<Issue> tmp = new List<Issue>();

                    foreach (var item in IssueList)
                    {
                        if (item.Status == issueColumn)
                        {
                            tmp.Add(item);
                        }
                    }

                    board.BoardColumns[i].ColumnIssues = tmp;
                    board.BoardColumns[i].IssueCount = tmp.Count;
                }


                int maxIssues = 0;
                for (int i = 0; i < board.BoardColumns.Count(); i++)
                {
                    if (board.BoardColumns[i].ColumnIssues.Count() > maxIssues)
                    {
                        maxIssues = board.BoardColumns[i].ColumnIssues.Count();
                    }
                }


                board.MaxIssueCount = maxIssues;

            }



            var asd = "";


            foreach (var item in BoardList)
            {
                asd += item.ID + " | " + item.ColumnCount + " | " + item.MaxIssueCount + " ";

                foreach (var n in item.BoardColumns)
                {
                    asd += n.IssueCount + Environment.NewLine;
                }
                asd += "||";
            }

            ViewData["js"] = asd;

            //*************************************************************************************************************

            ViewBag.BoardList = BoardList;


            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
