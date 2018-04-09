using System.Linq;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for filling board objects with appropriate information.
    /// </summary>
    public class BoardCreator : IBoardCreator
    {
        private readonly IJiraClient jiraClient;

        public BoardCreator(IJiraClient jiraClient)
        {
            this.jiraClient = jiraClient;
        }

        /// <summary>
        /// Helper function for retrieving a board from cache.
        /// </summary>
        /// <param name="id">Id of the board to be retrieved.</param>
        /// <param name="cache">In-memory cache where previously displayed board objects are stored.</param>
        /// <returns>
        /// The requested board from cache or a newly made empty board with the requested id.
        /// </returns>
        private Board TryGetBoardFromCache(int id, IMemoryCache cache)
        {
            if (!cache.TryGetValue(id, out Board cachedBoard))
            {
                return new Board(id);
            }
            else
            {
                cachedBoard.FromCache = true;
                return cachedBoard;
            }
        }

        /// <summary>
        /// Creates and fills a board object with appropriate information.
        /// </summary>
        /// <param name="id">Id of the board whose object will be made.</param>
        /// <param name="cache">In-memory cache where previously displayed board objects are stored.</param>
        /// <returns>A filled board object.</returns>
        public async Task<Board> CreateBoardModel(int id, string credentials, IMemoryCache cache)
        {
            var board = new Board(id);

            var boardConfig = await jiraClient.GetBoardDataAsync<BoardConfig>("agile/1.0/board/" + id.ToString() + "/configuration", credentials, id);

            var colorList = new ColorList();
            colorList = await jiraClient.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/" + id.ToString() + "/strategy/priority", credentials, id);

            if (boardConfig == null)  //
            {
                return TryGetBoardFromCache(id, cache);
            }

            board.Name = boardConfig.Name;
            board.CardColors = colorList.CardColors;

            FullIssueList li = new FullIssueList();
            IssueList issueList = await jiraClient.GetBoardDataAsync<IssueList>("agile/1.0/board/" + id.ToString() + "/issue", credentials, id);
            if (issueList == null)  //
            {
                return TryGetBoardFromCache(id, cache);
            }

            li.AllIssues.AddRange(issueList.Issues);
            while (issueList.StartAt + issueList.MaxResults < issueList.Total)
            {
                issueList.StartAt += issueList.MaxResults;
                issueList = await jiraClient.GetBoardDataAsync<IssueList>("agile/1.0/board/" + id.ToString() + "/issue?startAt=" + issueList.StartAt.ToString(), credentials, id);
                if (issueList == null)  //
                {
                    return TryGetBoardFromCache(id, cache);
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

            //sort issues in column by priority
            foreach (BoardColumn column in board.Columns)
            {
                column.Issues = column.Issues.OrderBy(a => a.Fields.Priority.Id).ToList();
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
