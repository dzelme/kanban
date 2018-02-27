using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class BoardRow : IEquatable<BoardRow>
    {
        public List<Issue> IssueRow { get; set; }

        public BoardRow()
        {
            IssueRow = new List<Issue>();
        }

        #region Equality

        public bool Equals(BoardRow other)
        {
            bool issueRowsEqual = false;
            if (IssueRow.Count != other.IssueRow.Count) { return false; }
            else
            {
                for (int i = 0; i < IssueRow.Count; i++)
                {
                    if (!IssueRow[i].Equals(other.IssueRow[i])) { return false; }
                }
                issueRowsEqual = true;
            }

            if (other == null) { return false; }
            return issueRowsEqual;  //IssueRow.SequenceEqual(other.IssueRow);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as BoardRow);
        }

        #endregion
    }
}
