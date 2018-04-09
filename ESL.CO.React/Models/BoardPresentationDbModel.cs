using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ESL.CO.React.Models
{
    public class BoardPresentationDbModel
    {
        [BsonId]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public Credentials Credentials { get; set; }
        public List<BoardDbModel> Boards { get; set; }

        public BoardPresentationDbModel()
        {
            Id = "";
            Title = "";
            Owner = "";
            Credentials = null;
            Boards = null;
        }
    }
}
