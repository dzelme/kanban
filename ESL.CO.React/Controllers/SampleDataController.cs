using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;

namespace ESL.CO.React.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        //Obtain board list from Jira, passes to React
        [HttpGet("[action]")]
        public async Task<IEnumerable<Models.Value>> BoardList()
        {
            var client = new JiraClient();
            var boardList = await client.GetBoardListAsync("board/");

            return boardList.Values;
        }

        //obtain a full kanban board
        [HttpGet("[action]")]
        public async Task<Models.Board> BoardData()
        {
            int id = 963;
            var client = new JiraClient();
            var boardConfig = await client.GetBoardConfigAsync("board/" + id.ToString() + "/configuration");
            FullIssueList li = new FullIssueList();
            IssueList issueList = await client.GetIssueListAsync("board/" + id.ToString() + "/issue");
            li.AllIssues.AddRange(issueList.Issues);
            while (issueList.StartAt + issueList.MaxResults < issueList.Total)
            {
                issueList.StartAt += issueList.MaxResults;
                issueList = await client.GetIssueListAsync("board/" + id.ToString() + "/issue?startAt=" + issueList.StartAt.ToString());
                li.AllIssues.AddRange(issueList.Issues);
            }

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

            //find number of rows in table (maximum)
            int rowCount = 0;
            foreach (BoardColumn c in board.Columns)
            {
                if (c.Issues.Count() > rowCount) { rowCount = c.Issues.Count(); }
            }

            //create a list of issues (for <td>) for each board row (<tr>) 
            for (int i = 0; i < rowCount; i++)
            {
                board.Rows.Add(new BoardRow());
                for (int j = 0; j < board.Columns.Count(); j++)
                {
                    if (board.Columns[j].Issues.ElementAtOrDefault(i) != null)  //checks if issue exists
                    {
                        board.Rows[i].IssueRow.Add(board.Columns[j].Issues[i]);  //adds issue to row
                    }
                    else
                    {
                        //if need to use this, make it so that new Issue cascades, creates new objects for property objects
                        board.Rows[i].IssueRow.Add(new Issue());  //creates empty issues, where there are none (can be removed)
                    }
                }
            }

            return board;
        }
    }
}
