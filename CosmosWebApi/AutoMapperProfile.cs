using AutoMapper;
using CosmosWebApi.DataObjects;
using CosmosWebApi.DataTransferObjects;

namespace CosmosWebApi
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();
        }
    }
}