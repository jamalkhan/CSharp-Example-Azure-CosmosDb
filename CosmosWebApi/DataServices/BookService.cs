using CosmosWebApi.DataObjects;
using CosmosWebApi.Settings;

namespace CosmosWebApi.DataServices
{

    public class BookService : BaseService<Book>, IDataService<Book>
    {
        public BookService(CosmosDbSettings settings) : base(settings, settings.BooksCollectionName)
        {
        }
    }
}