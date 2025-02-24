using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProfileService.Models
{
    public class History
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string id_user { get; set; }
        public string article_id { get; set; }
        public DateTime date {  get; set; }

    }

    public class Tag {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        public string name { get; set; }
        public List<string> articles { get; set; }
    }
}
