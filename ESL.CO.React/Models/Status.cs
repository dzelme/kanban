using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Status : IEquatable<Status>
    {
        public string Id { get; set; }  //which column belongs to
        public string Name { get; set; }  //e.g., to do, backlog

        public Status()
        {
            Name = string.Empty;
            Id = string.Empty;
        }

        #region Equality

        public bool Equals(Status other)
        {
            if (other == null)
            {
                if (this == null) { return true; }
                return false;
            }
            return string.Equals(Name, other.Name) &&
                string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Status);
        }

        #endregion
    }
}
