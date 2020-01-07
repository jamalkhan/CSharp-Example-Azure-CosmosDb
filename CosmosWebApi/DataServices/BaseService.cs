using CosmosWebApi.DataObjects;
using CosmosWebApi.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmosWebApi.DataServices
{
    public class CollectionStats<TDataEntity>
        : ICollectionStats<TDataEntity>
        where TDataEntity : class, IDataEntity
    {
        public CollectionStats(IEnumerable<TDataEntity> data, int skip, int page, int totalCount, int pageCount)
        {
            Returned = new ActualResults();
            Returned.Count = data.Count();
            Returned.FirstRecord = skip;
            Returned.LastRecord = Returned.FirstRecord + Returned.Count - 1;
            Returned.Page = page;
            Totals = new DataSetTotals();
            Totals.Pages = pageCount;
            Totals.Records = totalCount;
            Results = data;
        }
        public IEnumerable<TDataEntity> Results { get; set; }
        public ActualResults Returned { get; private set; }
        public DataSetTotals Totals { get; private set; }

    }

    public interface ICollectionStats<TDataEntity>
    {
        IEnumerable<TDataEntity> Results { get; set; }
        ActualResults Returned { get; }
        DataSetTotals Totals { get; }
    }

    public class ActualResults
    {
        public int Page { get; set; }
        public int Count { get; set; }
        public int FirstRecord { get; set; }
        public int LastRecord { get; set; }
    }

    public abstract class BaseService<TDataEntity>
        : IDataService<TDataEntity>
        where TDataEntity : class, IDataEntity
    {
        public readonly IMongoCollection<TDataEntity> _entities;
        public BaseService(CosmosDbSettings settings, string collectionName)
        {
            var client = new MongoClient(settings.MongoConnectionString);
            var database = client.GetDatabase(settings.MongoDatabaseName);
            _entities = database.GetCollection<TDataEntity>(collectionName);
        }

        public virtual ICollectionStats<TDataEntity> Get(int page, int pageSize)
        {
            var skip = page * pageSize;
            var data = _entities
                .AsQueryable()
                .Skip(skip)
                .Take(pageSize);

            var recordCount = _entities.AsQueryable().Count();
            var totalPages = (int)Math.Ceiling((double)recordCount / pageSize);

            var results = new CollectionStats<TDataEntity>
                                (
                                    data: data,
                                    skip: skip,
                                    page: page,
                                    totalCount: recordCount,
                                    pageCount: totalPages
                                );
            return results;
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