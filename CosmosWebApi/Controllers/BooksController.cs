using AutoMapper;
using CosmosWebApi.DataObjects;
using CosmosWebApi.DataServices;
using CosmosWebApi.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace CosmosWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : CrudControllerBase<BookService, BookDto, Book>
    {
        public BooksController(BookService service, IMapper mapper) : base(service, mapper)
        {
        }
    }
}