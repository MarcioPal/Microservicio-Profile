

using AutoMapper;
using ProfileService.DTO;

namespace ProfileService.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DTO.DtoProfile, Models.Profile>(); // Mapea ProductDto a Product
            CreateMap<Models.Profile, DTO.DtoProfile>(); // Mapea Product a ProductDto
            CreateMap<DTO.DtoHistory, Models.History>();
            CreateMap<Models.History, DTO.DtoHistory>();
            CreateMap<DTO.DtoUpdHistory, Models.History>();
        }
    }

}
