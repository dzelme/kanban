using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ESL.CO.React.Models
{
    public class Issue : IEquatable<Issue>
    {
        [JsonProperty(Required = Required.Always)]
        public string Key { get; set; }

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
