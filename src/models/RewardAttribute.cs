using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace src.models
{
    [BsonIgnoreExtraElements]
    [MongoCollection("rewardAttributes")]
    public class RewardAttribute
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [Required]
        public string Countries { get; set; }
        [Required]
        public string MinAge { get; set; } 
        [Required]
        public string MaxAge { get; set; }
        [Required]
        public int NumberOfUsersAbleToClaim { get; set; }
        [Required]
        public string RewardLink { get; set; }
        [Required]
        public string WalletAddress { get; set; }
        [Required]
        public float AmountPaidPerClick { get; set; }
        [Required]
        public float RewardAmount { get; set; }
    }
}
