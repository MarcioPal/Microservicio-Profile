using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProfileService.Models
{
    public class Profile
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)]
        [BindNever]
        public string? id { get; set; }
        [BindNever]
        public string? userId { get; set; }
        public string name { get; set; }
        public string lastname { get; set; } 
        public string birthdate { get; set; }
        public string phone { get; set; }
        public string? imageId { get; set; }
        public string? tag_id { get; set; }
        public Address? address { get; set;}


    }

    public class Address { 
        public string country { get; set; }
        public string province { get; set; }
        public string postcode { get; set; }
        public string city { get; set; }    
        public string street { get; set; }
    }
}
