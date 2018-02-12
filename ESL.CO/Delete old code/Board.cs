using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;

namespace ESL.CO.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board
    
    public class Board
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        //public string Link { get; set; }
        //public ColumnList Columns { get; set; }
        //public int MaxIssueCount { get; set; }
        public List<Column> Columns { get; set; }  //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/configuration





        public int IssueCount { get; set; }
        public int ColumnCount { get; set; }
        /*
        private int boardId;
        private int columnCount;
        private int issueCount;
        private List<Column> columns;
        //private JsonFile json;  //
        */
        
            /*
        //returns data from JIRA REST api
        public static JObject Connect(string url)
        {
            // HttpClient



            //string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue";
            string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/" + url;

            WebRequest myReq = WebRequest.Create(urlIssue);
            #region Credentials
            string credentials = "adzelme:testTEST0";
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
            this.Id = id;

            JObject j = Connect("board/" + Id + "/issue");
            IssueCount = (int)j.SelectToken("total");

            //testing...
            //j = Connect("board/" + BoardId + "/issue");
            //Json = new JsonFile(boardId, j.ToString(Newtonsoft.Json.Formatting.None));

            //string urlConfig = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + board + "/configuration";
            j = Connect("board/" + Id + "/configuration");
            var columnArray = j.SelectToken("columnConfig.columns");
            ColumnCount = columnArray.Count();
            Columns = new List<Column>();
            foreach (JObject column in columnArray)
            {
                //Column temp = new Column((string)column.GetValue("name")); //returns column name
                //Columns.Add(temp);
            }
        }

        //reads <= maxResults issues starting from startAt
        public string ReadBoardPage(int startAt = 0)
        {
            //string urlIssue = "https://jira.returnonintelligence.com/rest/agile/1.0/board/" + BoardId + "/issue?startAt=" + startAt;
            JObject j = Connect("board/" + Id + "/issue?startAt=" + startAt);

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
                    if (String.Equals(status, Columns[ii].Name, StringComparison.OrdinalIgnoreCase)) { t = 1; appropriateColumn = ii; }
                    if (String.Equals(status2, Columns[ii].Name, StringComparison.OrdinalIgnoreCase)) { t = 2; appropriateColumn = ii; }  //
                }
                if (t == 2) { status = status2; } //resolves status name location ambiguity

                if (id == null) { return j.ToString(Newtonsoft.Json.Formatting.None); }  //ends when no more issues to read

                //creates object adds to table
                Issue temp = new Issue(id, "key", status, priority, assignee, summary, link);  //key not read...
                //if (appropriateColumn != -1) { kanbanBoard[appropriateColumn].Add(temp); }
                Columns[appropriateColumn].Issues.Add(temp);
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

        */

        /// <summary>
        /// ///////////////////////////////////////////////////////
        /// </summary>
    }
    /*
    public class ColumnStatus
    {
        public string Id { get; set; }
        //public string self { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public List<ColumnStatus> Statuses { get; set; }  //which issues shown in this column
        public int? Max { get; set; }  //max number of issues in one column
    }

    public class ColumnConfig
    {
        public List<Column> Columns { get; set; }
        //public string constraintType { get; set; }
    }

    public class BoardConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        //public string self { get; set; }
        //public Filter filter { get; set; }
        //public SubQuery subQuery { get; set; }
        public ColumnConfig ColumnConfig { get; set; }
        //public Ranking ranking { get; set; }
    }
    */

}
