using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ESL.CO.React.Models
{
    public class Fields : IEquatable<Fields>
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
        [JsonProperty(Required = Required.Always)]
        public Priority Priority { get; set; }
        //public List<object> labels { get; set; }
        //public object aggregatetimeoriginalestimate { get; set; }
        //public object timeestimate { get; set; }
        //public List<object> versions { get; set; }
        //public List<object> issuelinks { get; set; }  //?
        public Assignee Assignee { get; set; }
        //public DateTime updated { get; set; }
        [JsonProperty(Required = Required.Always)]
        public Status Status { get; set; }  //which column belongs to
        //public List<Component> components { get; set; }
        //public object timeoriginalestimate { get; set; }
        public string Description { get; set; }
        //public Timetracking timetracking { get; set; }
        //public object aggregatetimeestimate { get; set; }
        //public bool flagged { get; set; }
        [JsonProperty(Required = Required.Always)]
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

        #region Equality

        //public static bool operator == (Fields a, Fields b)
        //{
        //    return a.Equals(b);
        //}

        //public static bool operator != (Fields a, Fields b)
        //{
        //    return a.Equals(b);
        //}
        
        public bool Equals(Fields other)
        {
            if (other == null)
            {
                if (this == null) { return true; }
                return false;
            }

            bool assEqual = false;
            if (Assignee != null) { assEqual = Assignee.Equals(other.Assignee); }
            else
            {
                if (other.Assignee == null) { assEqual = true; }
                else { assEqual = false; }
            }
            bool priorityEqual = false;
            if (Priority != null) { priorityEqual = Priority.Equals(other.Priority); }
            else
            {
                if (other.Priority == null) { priorityEqual = true; }
                else { priorityEqual = false; }
            }
            bool statusEqual = false;
            if (Status != null) { statusEqual = Status.Equals(other.Status); }
            else
            {
                if (other.Status == null) { statusEqual = true; }
                else { statusEqual = false; }
            }

            return priorityEqual && //Priority.Equals(other.Priority) &&
                assEqual  &&
                statusEqual && //Status.Equals(other.Status) &&
                string.Equals(Description, other.Description) &&
                string.Equals(Summary, other.Summary);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Fields);
        }

        #endregion
    }
}
