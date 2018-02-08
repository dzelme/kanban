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

namespace ESL.CO.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Nolasītā informācija:";

            string BoardId;

            string credentials = "arumka:Dzukste22";


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
                asd += item.ID + " | " + item.ColumnCount + " | " + item.MaxIssueCount +  " ";

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
