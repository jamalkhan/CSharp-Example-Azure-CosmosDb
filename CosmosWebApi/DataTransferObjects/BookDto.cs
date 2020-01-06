namespace CosmosWebApi.DataTransferObjects
{
    public class BookDto : IDtoEntity
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
    }
}