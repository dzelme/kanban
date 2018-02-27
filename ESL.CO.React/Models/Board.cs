using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Board : IEquatable<Board>
    {
        public int Id { get; set; }
        public string Name { get;set; }
        public bool FromCache { get; set; }  //
        public string Message { get; set; }  //
        public List<BoardColumn> Columns { get; set; }
        public List<BoardRow> Rows { get; set; }  //

        public Board(int id = 0, bool fromCache = false, string message = "")
        {
            Id = id;
            Name = string.Empty;
            this.FromCache = fromCache; 
            this.Message = message;
            Columns = new List<BoardColumn>();
            Rows = new List<BoardRow>();
        }

        #region Equality

        public bool Equals(Board other)
        {
            if (this == null) { return false; }  //
            if (other == null) { return false; }

            bool columnsEqual = false;
            if (Columns.Count != other.Columns.Count) { return false; }
            else
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (!Columns[i].Equals(other.Columns[i])) { return false; }
                }
                columnsEqual = true;
            }

            bool result = Id == other.Id &&
                string.Equals(Name, other.Name) &&
                FromCache == other.FromCache &&
                string.Equals(Message, other.Message) &&
                columnsEqual;   //Columns.SequenceEqual(other.Columns);// &&
                //Rows.SequenceEqual(other.Rows);  //not needed cause same as columns
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Board);
        }

        #endregion
    }
}
