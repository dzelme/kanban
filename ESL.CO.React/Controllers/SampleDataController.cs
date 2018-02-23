using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;


namespace ESL.CO.React.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        //Obtain board list from Jira, passes to React
        [HttpGet("[action]")]
        public async Task<IEnumerable<Models.Value>> BoardList()
        {
            /*
            var client = new JiraClient();
            var boardList = await client.GetBoardDataAsync<BoardList>("board/");

            return boardList.Values;
            */


            var client = new JiraClient();
            var a = new AppSettings();
            var boardList = await client.GetBoardDataAsync<BoardList>("board/");
            if (boardList == null)
            {
                return a.GetSavedAppSettings()?.AllValues;
            }  //


            FullBoardList appSettings = new FullBoardList();
            appSettings.AllValues.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await client.GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString());
                //if (boardList == null) { return null; }  //
                if (boardList == null)
                {
                    appSettings = a.MergeSettings(a.GetSavedAppSettings(), appSettings);
                    a.SaveAppSettings(appSettings);
                    return appSettings.AllValues;
                }
                appSettings.AllValues.AddRange(boardList.Values);
            }

            appSettings = a.MergeSettings(a.GetSavedAppSettings(), appSettings);
            a.SaveAppSettings(appSettings);
            return appSettings.AllValues;
        }

        //obtain a full kanban board
        [HttpGet("[action]")]
        public async Task<Models.Board> BoardData(int id)
        {
            var creator = new BoardCreator();
            var b = creator.CreateBoardModel(id);
            var board = await b;

            if (board.FromCache == true) { return board; }  // in case of failed connection. currently draws. should do nothing instead...

            var cache = new CacheMethods();
            if (cache.NeedsRedraw(board)) { return board; }  // new board different. draws.

            //return cache.GetCachedBoard(board.Id);  //shouldn't redraw from cache. should do nothing instead.....
            return board;

            #region
            /*
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
                        //creates empty issues, where there are none (without this issues allign to left in wrong columns)
                        board.Rows[i].IssueRow.Add(new Issue());
                    }
                }
            }
            */

            /////////
            /*
            // read cached file
            // read from JSON to object, if file exists
            var filePath = @".\data\" + id.ToString() + ".json";  //use path.combine...
            var cachedHash = string.Empty;
            Board cachedBoard = new Board();
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    cachedBoard = JsonConvert.DeserializeObject<Board>(json);
                }
                cachedHash = GetHashCode(filePath, new MD5CryptoServiceProvider());
            }
            else
            { }
            
            // save info read from JIRA in a temp file
            // serialize JSON directly to a file
            var tempPath = @".\data\temp.json";
            using (StreamWriter file = System.IO.File.CreateText(tempPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, board);
            }
            var hash = GetHashCode(tempPath, new MD5CryptoServiceProvider());


            // overwrite cached file if new file is different
            if (!String.Equals(cachedHash, hash))
            {
                System.IO.File.Copy(tempPath, filePath, true);
                System.IO.File.Delete(tempPath);
                return board;
            }

            System.IO.File.Delete(tempPath);
            return cachedBoard;  //shouldn't redraw from cache. should do nothing instead.....
            */
            #endregion
        }

        [HttpGet("[action]/{id?}")]
        public Value BoardConfig(int id)
        {
            //var id = int.Parse(Request.QueryString.ToString());
            //var id = RouteParameter.Optional;
            var a = new AppSettings();
            var appSettings = a.GetSavedAppSettings();
            var boardConfig = new Value();
            foreach (var val in appSettings.AllValues)
            {
                if (val.Id == id) { boardConfig = val; }
            }
            return boardConfig;
        }
    }
}
