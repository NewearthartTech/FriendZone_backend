using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace src.models
{
    [BsonIgnoreExtraElements]
    [MongoCollection("verifiers")]
    public class Verifier
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; } = "";
        public string Challenge { get; set; } = "";
        public string walletAddress { get; set; } = "";
    }
}
