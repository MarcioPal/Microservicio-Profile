

using AutoMapper;
using ProfileService.DTO;

namespace ProfileService.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DtoProfile, Profile>(); // Mapea ProductDto a Product
            CreateMap<Profile, DtoProfile>(); // Mapea Product a ProductDto
            CreateMap<DtoHistory, Models.History>();
            CreateMap<Models.History, DtoHistory>();
            CreateMap<DtoUpdHistory, Models.History>();
        }
    }

}
