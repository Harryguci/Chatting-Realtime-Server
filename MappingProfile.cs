using AutoMapper;
using ChatingApp.Models;
using ChatingApp.Models.Dtos;

namespace ChatingApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}
