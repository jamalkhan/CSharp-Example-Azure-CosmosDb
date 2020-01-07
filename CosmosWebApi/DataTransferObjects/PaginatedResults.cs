using System.Collections.Generic;

namespace CosmosWebApi.DataTransferObjects
{
    public class PaginatedResults<TDtoEntity>
        where TDtoEntity : IDtoEntity
    {
        public ActualResults Returned { get; set; } = new ActualResults();
        public DataSetTotals Totals { get; set; } = new DataSetTotals();
        public class ActualResults
        {
            public int Page { get; set; }
            public int Count { get; set; }
            public int FirstRecord { get; set; }
            public int LastRecord { get; set; }
        }
        public class DataSetTotals
        {
            public int Pages { get; set; }
            public int Records { get; set; }
        }
        public IEnumerable<TDtoEntity> Results { get; set; }
    }
}