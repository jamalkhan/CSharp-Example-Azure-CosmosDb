using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CosmosWebApi.DataObjects
{
    public class Book : IDataEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
    }
}