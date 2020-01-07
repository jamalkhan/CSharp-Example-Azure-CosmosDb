using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosWebApi.Settings
{
    public class CosmosDbSettings
    {
        public bool IsCosmosDb { get; set; }
        public string MongoHost { get; set; }
        public int MongoPort { get; set; }
        public string MongoUser { get; set; }
        public string MongoPassword { get; set; }
        public string MongoDatabaseName { get; set; }
        public string BooksCollectionName { get; set; }
    }
}
