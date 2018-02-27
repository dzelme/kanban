using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Priority : IEquatable<Priority>
    {
        //public string self { get; set; }
        //public string iconUrl { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }  //e.g., critical
        
        public Priority()
        {
            Name = string.Empty;
            Id = string.Empty;
        }

        #region Equality

        //public static bool operator == (Priority a, Priority b)
        //{
        //    return a.Equals(b);
        //}

        //public static bool operator != (Priority a, Priority b)
        //{
        //    return a.Equals(b);
        //}

        public bool Equals(Priority other)
        {
            if (other == null)
            {
                if (this == null) { return true; }
                return false;
            }
            return string.Equals(Id, other.Id) &&
                string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Priority);
        }

        #endregion
    }
}
