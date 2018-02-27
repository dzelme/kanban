using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Issue : IEquatable<Issue>
    {
        //public string expand { get; set; }
        //public string id { get; set; }
        //public string self { get; set; }
        public string Key { get; set; }  //used to generate link to the particular issue
        public Fields Fields { get; set; }

        public Issue()
        {
            Key = string.Empty;
            Fields = new Fields();
        }

        #region Equality

        public bool Equals(Issue other)
        {
            if (other == null) { return false; }
            return string.Equals(Key, other.Key) &&
                Fields.Equals(other.Fields);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Issue);
        }

        #endregion
    }

}
