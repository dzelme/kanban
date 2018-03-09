using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;

using Microsoft.Extensions.Caching.Memory;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for filling board objects with appropriate information.
    /// </summary>
    public class BoardCreator
    {
        private readonly IJiraClient jiraClient;

        public BoardCreator(IJiraClient jiraClient)
        {
            this.jiraClient = jiraClient;
        }

        /// <summary>
        /// Creates and fills a board object with appropriate information.
        /// </summary>
        /// <param name="id">Id of the board whose object will be made.</param>
        /// <param name="cache">In-memory cache where previously displayed board objects are stored.</param>
        /// <returns>A filled board object.</returns>
        public async Task<Board> CreateBoardModel(int id, IMemoryCache cache)
        {
            var board = new Board(id);
            //var cache = new CacheMethods();
            var boardConfig = await jiraClient.GetBoardDataAsync<BoardConfig>("board/" + id.ToString() + "/configuration", id);
            if (boardConfig == null)  //
            {
                if (!cache.TryGetValue(id, out Board cachedBoard))
                {
                    return new Board(id);
                }
                cachedBoard.FromCache = true;
                return cachedBoard;
            }

            board.Name = boardConfig.Name;

            FullIssueList li = new FullIssueList();
            IssueList issueList = await jiraClient.GetBoardDataAsync<IssueList>("board/" + id.ToString() + "/issue", id);
            if (issueList == null)  //
            {
                if (!cache.TryGetValue(id, out Board cachedBoard))
                {
                    return new Board(id);
                }
                cachedBoard.FromCache = true;
                return cachedBoard;
            }

            li.AllIssues.AddRange(issueList.Issues);
            while (issueList.StartAt + issueList.MaxResults < issueList.Total)
            {
                issueList.StartAt += issueList.MaxResults;
                issueList = await jiraClient.GetBoardDataAsync<IssueList>("board/" + id.ToString() + "/issue?startAt=" + issueList.StartAt.ToString(), id);
                if (issueList == null)  //
                {
                    if (!cache.TryGetValue(id, out Board cachedBoard))
                    {
                        return new Board(id);
                    }
                    cachedBoard.FromCache = true;
                    return cachedBoard;
                }
                li.AllIssues.AddRange(issueList.Issues);
            }

            //create a board model with issues assigned to appropriate column
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
                        //creates empty issues, where there are none (without this issues allign to left in wrong columns)
                        board.Rows[i].IssueRow.Add(new Issue());
                    }
                }
            }

            return board;
        }
    }
}
