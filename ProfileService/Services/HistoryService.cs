using AutoMapper;
using ProfileService.DTO;
using ProfileService.Models;

namespace ProfileService.Services
{
    
    public class HistoryService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IndireccionAuthService _indireccionAuthService;
        private readonly IMapper _mapper;

        public HistoryService(MongoDbService _mongoDbService, IndireccionAuthService indireccionAuthService, IMapper mapper)
        {
            this._mongoDbService = _mongoDbService;
            this._indireccionAuthService = indireccionAuthService;
            this._mapper = mapper;
        }


        public async Task<bool> updateHistory(DtoUpdHistory dto)
        {

            if (dto.token is null)
            {
                throw new Exception("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(dto.token);
            if (user is not null)
            {
                History history = _mapper.Map<History>(dto);
                if (!await _mongoDbService.existsHistoryByUserIdAndArtId(user.id, dto.article_id))
                {

                    history.id_user = user.id;

                    await _mongoDbService.newHistoryAsync(history);

                    return true;
                }
                await _mongoDbService.updateHistoryDate(history);
                return true;
            }
            throw new Exception("Error: Token invalido");


        }
    }
}
