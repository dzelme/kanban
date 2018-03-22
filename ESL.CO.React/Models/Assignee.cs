using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Assignee : IEquatable<Assignee>
    {
        public string DisplayName { get; set; } 

        public Assignee()
        {
            DisplayName = string.Empty;
        }

        #region Equality

        //public static bool operator == (Assignee a, Assignee b)
        //{                              
        //    return a.Equals(b);        
        //}                              
                                       
        //public static bool operator != (Assignee a, Assignee b)
        //{
        //    return a.Equals(b);
        //}

        public bool Equals(Assignee other)
        {
            if (other == null)
            {
                if (this == null) { return true; }
                return false;
            }
            return string.Equals(DisplayName, other.DisplayName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Assignee);
        }

        #endregion
    }
}
