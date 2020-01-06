using CosmosWebApi.DataObjects;
using CosmosWebApi.Settings;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace CosmosWebApi.DataServices
{

    public abstract class BaseService<TDataEntity>
        : IDataService<TDataEntity>
        where TDataEntity : IDataEntity
    {
        public readonly IMongoCollection<TDataEntity> _entities;
        public BaseService(CosmosDbSettings settings, string collectionName)
        {
            var client = new MongoClient(settings.MongoConnectionString);
            var database = client.GetDatabase(settings.MongoDatabaseName);
            _entities = database.GetCollection<TDataEntity>(collectionName);
        }

        public virtual List<TDataEntity> Get()
        {
            return _entities.Find(entity => true).ToList();
        }

        public virtual TDataEntity Get(string id)
        {
            return _entities.Find<TDataEntity>(entity => entity.Id == id).FirstOrDefault();
        }
        public virtual TDataEntity Create(TDataEntity entity)
        {
            _entities.InsertOne(entity);
            return entity;
        }


        public virtual void Update(string id, TDataEntity entityIn)
        {
            _entities.ReplaceOne(entity => entity.Id == id, entityIn);
        }

        public virtual void Remove(TDataEntity entityIn)
        {
            _entities.DeleteOne(entity => entity.Id == entityIn.Id);
        }


        public virtual void Remove(string id)
        {
            _entities.DeleteOne(entity => entity.Id == id);
        }

    }
}