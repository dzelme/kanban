using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Fields
    {
        //public Issuetype issuetype { get; set; }
        //public object timespent { get; set; }
        //public Project project { get; set; }
        //public List<object> fixVersions { get; set; }
        //public object aggregatetimespent { get; set; }
        //public object resolution { get; set; }
        //public object resolutiondate { get; set; }
        //public int workratio { get; set; }
        //public DateTime? lastViewed { get; set; }
        //public Watches watches { get; set; }
        //public DateTime created { get; set; }
        public Priority Priority { get; set; }
        //public List<object> labels { get; set; }
        //public object aggregatetimeoriginalestimate { get; set; }
        //public object timeestimate { get; set; }
        //public List<object> versions { get; set; }
        //public List<object> issuelinks { get; set; }  //?
        public Assignee Assignee { get; set; }
        //public DateTime updated { get; set; }
        public Status Status { get; set; }  //which column belongs to
        //public List<Component> components { get; set; }
        //public object timeoriginalestimate { get; set; }
        public string Description { get; set; }
        //public Timetracking timetracking { get; set; }
        //public object aggregatetimeestimate { get; set; }
        //public bool flagged { get; set; }
        public string Summary { get; set; }
        //public Creator creator { get; set; }
        //public List<object> subtasks { get; set; }
        //public Reporter reporter { get; set; }
        //public Aggregateprogress aggregateprogress { get; set; }
        //public object environment { get; set; }
        //public object duedate { get; set; }
        //public Progress progress { get; set; }
        //public Comment comment { get; set; }
        //public Votes votes { get; set; }
        //public Worklog worklog { get; set; }

        public Fields()
        {
            Priority = new Priority();
            Assignee = new Assignee();
            Status = new Status();
            Description = string.Empty;
            Summary = string.Empty;
        }
    }
}
