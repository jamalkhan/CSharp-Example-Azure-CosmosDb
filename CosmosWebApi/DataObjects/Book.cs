namespace CosmosWebApi.DataObjects
{
    public class Book : IDataEntity
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
    }
}