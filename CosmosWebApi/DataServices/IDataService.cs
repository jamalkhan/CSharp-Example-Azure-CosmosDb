﻿using CosmosWebApi.DataObjects;
using System.Collections.Generic;

namespace CosmosWebApi.DataServices
{
    public interface IDataService<TDataEntity>
        where TDataEntity : IDataEntity
    {
        TDataEntity Create(TDataEntity dataEntity);
        List<TDataEntity> Get();
        TDataEntity Get(string id);
        void Remove(TDataEntity dataEntity);
        void Remove(string id);
        void Update(string id, TDataEntity dataEntity);
    }
}
