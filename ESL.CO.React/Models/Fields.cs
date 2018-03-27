using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ESL.CO.React.Models
{
    public class Fields : IEquatable<Fields>
    {
        [JsonProperty(Required = Required.Always)]
        public Priority Priority { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Status Status { get; set; } 
   
        [JsonProperty(Required = Required.Always)]
        public string Summary { get; set; }

        public DateTime Created { get; set; }
        public Progress Progress { get; set; }
        public Assignee Assignee { get; set; }
        public string Description { get; set; }

        public Fields()
        {
            Priority = new Priority();
            Assignee = new Assignee();
            Status = new Status();
            Description = string.Empty;
            Summary = string.Empty;
            Progress = new Progress();
            Created = new DateTime();
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
