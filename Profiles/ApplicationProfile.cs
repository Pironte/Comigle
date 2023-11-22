using AutoMapper;
using Comigle.Model;
using Comigle.Model.Dtos;

namespace Comigle.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<CreateUserDto, User>().ReverseMap();
        }
    }
}
