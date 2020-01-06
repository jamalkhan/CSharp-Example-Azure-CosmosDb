using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosWebApi.Settings
{
    public class CosmosDbSettings
    {
        public string MongoConnectionString { get; set; }
        public bool MongoUseTsl12 { get; set; }
        public string MongoDatabaseName { get; set; }
        public string BooksCollectionName { get; set; }
    }
}
