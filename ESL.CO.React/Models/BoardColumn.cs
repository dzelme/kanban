using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class BoardColumn : IEquatable<BoardColumn>
    {
        public string Name { get; set; }
        public List<Issue> Issues { get; set; }

        public BoardColumn(string name)
        {
            Name = name;
            Issues = new List<Issue>();
        }

        #region Equality

        public bool Equals (BoardColumn other)
        {
            bool issuesEqual = false;
            if (Issues.Count != other.Issues.Count) { return false; }
            else
            {
                for (int i = 0; i < Issues.Count; i++)
                {
                    if (!Issues[i].Equals(other.Issues[i])) { return false; }
                }
                issuesEqual = true;
            }

            if (other == null) { return false; }
            return string.Equals(Name, other.Name) &&
                issuesEqual;   
                //Issues.SequenceEqual(other.Issues);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as BoardColumn);
        }

        #endregion
    }
}
