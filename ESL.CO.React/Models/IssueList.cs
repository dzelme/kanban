using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/issue
    
/*
    public class Issuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }

    public class AvatarUrls
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }
*/
    public class Project
    {
        //public string self { get; set; }
        public string Id { get; set; }  //e.g., 15786
        public string Key { get; set; }  //e.g., KP
        public string Name { get; set; }  //e.g., KOSMOSS Prakse
        //public AvatarUrls avatarUrls { get; set; }
    }
/*
    public class Customfield10355
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Customfield10356
    {
        public string self { get; set; }
        public string value { get; set; }
        public string id { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }
*/
    public class Priority
    {
        //public string self { get; set; }
        //public string iconUrl { get; set; }
        public string Name { get; set; }  //e.g., critical
        public string Id { get; set; }
    }
/*
    public class AvatarUrls2
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }
*/

    public class Assignee
    {
        //public string self { get; set; }
        //public string name { get; set; }  //username
        //public string key { get; set; }
        //public string emailAddress { get; set; }
        //public AvatarUrls2 avatarUrls { get; set; }  //avatar image in 4 sizes (48x48, ..., 16x16)
        public string DisplayName { get; set; }  //full name
        //public bool active { get; set; }
        //public string timeZone { get; set; }
    }
/*
    public class StatusCategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }
*/

    public class Status
    {
        //public string self { get; set; }
        //public string description { get; set; }
        //public string iconUrl { get; set; }
        public string Name { get; set; }  //e.g., to do, backlog
        public string Id { get; set; }  //which column belongs to
        //public StatusCategory statusCategory { get; set; }
    }
/*
    public class Component
    {
        public string self { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Timetracking
    {
    }

    public class AvatarUrls3
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls3 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class AvatarUrls4
    {
        public string __invalid_name__48x48 { get; set; }
        public string __invalid_name__24x24 { get; set; }
        public string __invalid_name__16x16 { get; set; }
        public string __invalid_name__32x32 { get; set; }
    }

    public class Reporter
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string emailAddress { get; set; }
        public AvatarUrls4 avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Aggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Progress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Comment
    {
        public List<object> comments { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public int startAt { get; set; }
    }

    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Worklog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<object> worklogs { get; set; }
    }
*/
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
    }

    public class Issue
    {
        //public string expand { get; set; }
        //public string id { get; set; }
        //public string self { get; set; }
        public string Key { get; set; }  //used to generate link to the particular issue
        public Fields Fields { get; set; }

        public Issue()
        {
            Key = string.Empty;
            Fields = null;
        }
    }

    public class IssueList
    {
        //public string expand { get; set; }
        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }
        public List<Issue> Issues { get; set; }
    }

    public class FullIssueList //: IssueList
    {
        public List<Issue> AllIssues { get; set; }

        public FullIssueList()
        {
            AllIssues = new List<Issue>();
        }
    }
}



