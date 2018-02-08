using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;

namespace ESL.CO.Helpers
{
    public class Board
    {
        private int boardId;
        private int columnCount;
        private int issueCount;
        private List<Column> columns;
        //private JsonFile json;  //

        public int BoardId { get => boardId; set => boardId = value; }
        public int ColumnCount { get => columnCount; set => columnCount = value; }  //set not needed?
        public int IssueCount { get => issueCount; set => issueCount = value; }
        public List<Column> Columns { get => columns; set => columns = value; }
        //public JsonFile Json { get => json; set => json = value; }  //
        

        //returns data from JIRA REST api
        public JObject Connect(string url)
        {
            // HttpClient



            //string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue";
            string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/" + url;
            
            WebRequest myReq = WebRequest.Create(urlIssue);
            #region Credentials
            string credentials = "user:pass";
            #endregion
            myReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            WebResponse wr = myReq.GetResponse();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            JObject j = JObject.Parse(content);

            return j;
        }

        //gets board config
        public Board(int id)
        {
            boardId = id;
            
            JObject j = Connect("board/" + BoardId + "/issue");
            IssueCount = (int)j.SelectToken("total");

            //testing...
            //j = Connect("board/" + BoardId + "/issue");
            //Json = new JsonFile(boardId, j.ToString(Newtonsoft.Json.Formatting.None));

            //string urlConfig = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + board + "/configuration";
            j = Connect("board/" + BoardId + "/configuration");
            var columnArray = j.SelectToken("columnConfig.columns");
            columnCount = columnArray.Count();
            columns = new List<Column>();
            foreach (JObject column in columnArray)
            {
                Column temp = new Column((string)column.GetValue("name")); //returns column name
                columns.Add(temp);
            }
        }

        //reads <= maxResults issues starting from startAt
        public string ReadBoardPage(int startAt = 0)
        {
            //string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue?startAt=" + startAt;
            JObject j = Connect("board/" + BoardId + "/issue?startAt=" + startAt);

            int maxResults = 50; //make it a constant...
            for (int i = 0; i < maxResults; i++)  //(i < issueCount) - all issues, but max only 50 per request
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
                for (int ii = 0; ii < ColumnCount; ii++)
                {
                    if (String.Equals(status, columns[ii].Name, StringComparison.OrdinalIgnoreCase)) { t = 1; appropriateColumn = ii; }
                    if (String.Equals(status2, columns[ii].Name, StringComparison.OrdinalIgnoreCase)) { t = 2; appropriateColumn = ii; }  //
                }
                if (t == 2) { status = status2; } //resolves status name location ambiguity

                if (id == null) { return j.ToString(Newtonsoft.Json.Formatting.None); }  //ends when no more issues to read

                //creates object adds to table
                Issue temp = new Issue(id, status, priority, assignee, summary, link);
                //if (appropriateColumn != -1) { kanbanBoard[appropriateColumn].Add(temp); }
                columns[appropriateColumn].Issues.Add(temp);
            }
            //SaveToFile(j);
            return j.ToString(Newtonsoft.Json.Formatting.None);
            
        }

        //reads all issues from JIRA
        public void ReadBoard(int startAt = 0)
        {
            int maxResults = 50; //per request
            if (IssueCount <= (startAt + maxResults))
            {
                ReadBoardPage(startAt);
            }
            else
            {
                ReadBoardPage(startAt);
                ReadBoard(startAt + maxResults);
            }
            return;
        }
    }
}
