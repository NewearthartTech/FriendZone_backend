using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace src.models
{
    [BsonIgnoreExtraElements]
    [MongoCollection("referals")]
    public class Referal
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string PersonalLink { get; set; } = "";
        [Required]
        public string WalletAddress { get; set; }
        [Required]
        public string RewardLink { get; set; } 
    }
}
