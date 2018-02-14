using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ESL.CO.JiraIntegration;
using ESL.CO.Models;

namespace ESL.CO.React.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            //this.Id = id;
            //JObject j = Connect("board/" + Id + "/issue");

            var client = new JiraClient();
            var boardList = await client.GetBoardListAsync("board/");

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
            boardConfig = await client.GetBoardConfigAsync("board/" + id.ToString() + "/configuration");
            FullIssueList li = new FullIssueList();
            IssueList issueList = await client.GetIssueListAsync("board/" + id.ToString() + "/issue");
            li.AllIssues.AddRange(issueList.Issues);
            while (issueList.StartAt + issueList.MaxResults < issueList.Total)
            {
                issueList.StartAt += issueList.MaxResults;
                issueList = await client.GetIssueListAsync("board/" + id.ToString() + "/issue?startAt=" + issueList.StartAt.ToString());
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


            var ivm = new IndexViewModel();
            ivm.ColumnLength = columnLength;
            ivm.ColumnCount = board.Columns.Count();
            ivm.RowCount = rowCount;
            ivm.Columns = board.Columns;

            /*
            //viewbag no good
            ViewBag.columnLength = columnLength;  //boardConfig.ColumnConfig.Columns[].Issues.Count()
            ViewBag.columnCount = board.Columns.Count();
            ViewBag.rowCount = rowCount;
            ViewBag.columns = board.Columns; //boardConfig.ColumnConfig.Columns;
            */

            var asd = "";
            return View(ivm);
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
