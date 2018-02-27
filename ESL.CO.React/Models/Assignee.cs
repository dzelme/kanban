using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Assignee : IEquatable<Assignee>
    {
        //public string self { get; set; }
        //public string name { get; set; }  //username
        //public string key { get; set; }
        //public string emailAddress { get; set; }
        //public AvatarUrls2 avatarUrls { get; set; }  //avatar image in 4 sizes (48x48, ..., 16x16)
        public string DisplayName { get; set; }  //full name
        //public bool active { get; set; }
        //public string timeZone { get; set; }

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
