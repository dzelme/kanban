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

using ESL.CO.JiraIntegration;

//using ESL.CO.Helpers;

namespace ESL.CO.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //this.Id = id;
            //JObject j = Connect("board/" + Id + "/issue");

            var client = new JiraClient();
            var boardList = client.GetBoardListAsync("board/").Result;

            BoardConfig boardConfig = null;

            /*
            //loops thorough all kanban boards
            foreach (Value board in boardList.Values)
            {
                if (String.Equals(board.Type, "kanban", StringComparison.OrdinalIgnoreCase))
                {
                    //
                    boardConfig = client.GetBoardConfigAsync("board/" + board.Id + "/configuration").Result;
                }
            }
            */

            //get full list of all issues
            int id = 963;
            boardConfig = client.GetBoardConfigAsync("board/" + id.ToString() + "/configuration").Result;
            FullIssueList li = new FullIssueList();
            IssueList issueList = client.GetIssueListAsync("board/" + id.ToString() + "/issue").Result;
            li.AllIssues.AddRange(issueList.Issues);
            while (issueList.StartAt + issueList.MaxResults < issueList.Total)
            {
                issueList.StartAt += issueList.MaxResults;
                issueList = client.GetIssueListAsync("board/" + id.ToString() + "/issue?startAt=" + issueList.StartAt.ToString()).Result;
                li.AllIssues.AddRange(issueList.Issues);
            }


            /*
            var board = new List<List<Issue>>();
            for (int i = 0; i < boardConfig.ColumnConfig.Columns.Count(); i++)
            {
                var temp = new List<Issue>();
                board.Add(temp);
            }
            */

            //create a board model with issues assigned to appropriate column
            var board = new Board(id);
            foreach (Column col in boardConfig.ColumnConfig.Columns)
            {
                board.Columns.Add(new BoardColumn(col.Name));
            }


            //find appropriate column for each issue
            foreach (Issue issue in li.AllIssues)  //(Issue issue in issueList.Issues)
            {
                //foreach (Column col in boardConfig.ColumnConfig.Columns)
                for (int i = 0; i < boardConfig.ColumnConfig.Columns.Count(); i++)
                {
                    //foreach (var status in col.Statuses)
                    foreach (var status in boardConfig.ColumnConfig.Columns[i].Statuses)
                    {
                        if (status.Id == issue.Fields.Status.Id)
                        {
                            board.Columns[i].Issues.Add(issue);
                            //add issue to this column
                        }
                    }
                }
            }

            //finds number of rows in table (maximum)
            int rowCount = 0;
            foreach (BoardColumn c in board.Columns)
            {
                if (c.Issues.Count() > rowCount) { rowCount = c.Issues.Count(); }
            }

            //creates a helper array with number of issues in each column (column = index) required for passing to view
            var columnLength = new int[board.Columns.Count()];
            for (int i = 0; i < board.Columns.Count(); i++)
            {
                columnLength[i] = board.Columns[i].Issues.Count();
            }

            ViewBag.columnLength = columnLength;  //boardConfig.ColumnConfig.Columns[].Issues.Count()
            ViewBag.columnCount = board.Columns.Count();
            ViewBag.rowCount = rowCount;
            ViewBag.columns = board.Columns; //boardConfig.ColumnConfig.Columns;


            var asd = "";
            return View();
        }
        /*
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
        
       */

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
