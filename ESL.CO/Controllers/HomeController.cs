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
using ESL.CO.Helpers;

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
            
            int boardId = 620;  //620, 880, 961, 963
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

            
            return View();
        }

        public IActionResult AllBoards()
        {
            ViewData["Message"] = "Nolasītā informācija:";

            string BoardId;

            //*************************************************************************************************************
            //Visi dēļi

            JObject j = Helpers.Board.Connect("board/");

            var AllBoards = j.SelectToken("values");
            int BoardCount = AllBoards.Count();

            List<Models.Board> BoardList = new List<Models.Board>();

            foreach (JObject board in AllBoards)
            {
                Models.Board B = new Models.Board
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

            foreach (var board in BoardList)
            {
                List<Models.Column> ColumnList = new List<Models.Column>();

                BoardId = board.ID;

                j = Helpers.Board.Connect("board/" + BoardId.ToString() + "/configuration");

                var AllColumns = j.SelectToken("columnConfig.columns");

                foreach (JObject column in AllColumns)
                {
                    Models.Column C = new Models.Column
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

                List<Models.Issue> IssueList = new List<Models.Issue>();

                j = Helpers.Board.Connect("board/" + BoardId.ToString() + "/issue");

                var AllIssues = j.SelectToken("issues");

                foreach (var issue in AllIssues)
                {
                    Models.Issue I = new Models.Issue
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
                            Models.Issue I = new Models.Issue
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

                    List<Models.Issue> tmp = new List<Models.Issue>();

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
