using AutoMapper;
using Newtonsoft.Json.Linq;
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

        public async Task<List<History>> Get(string token) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                List<History> lista = await _mongoDbService.getHistoryAsync(user.id);
                return lista;
            }
            throw new UnauthorizedAccessException("Error: Token invalido");
        }

        public async Task<string> Save(string token, DtoHistory dtoHistory)
        {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                History history = _mapper.Map<History>(dtoHistory);
                if (!await _mongoDbService.existsHistoryByUserIdAndArtId(user.id, dtoHistory.article_id))
                {

                    history.id_user = user.id;

                    await _mongoDbService.newHistoryAsync(history);

                    return ("El articulo se ha añadido correctamente al historial de visualizaciones");
                }
                await _mongoDbService.updateHistoryDate(history);
                return ("Se ha actualizado el historial de articulos vistos");
            }
            throw new UnauthorizedAccessException("Error: Token invalido");
        }



        public async Task Delete(string token) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                await _mongoDbService.deleteAllHistoryByUser(user.id);
                return;
            }
            throw new UnauthorizedAccessException("Error: Token invalido");

        }


        public async Task<bool> Update(DtoUpdHistory dto)
        {

            if (dto.token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(dto.token);
            if (user is not null)
            {
                History history = _mapper.Map<History>(dto);
                history.id_user = user.id;
                if (!await _mongoDbService.existsHistoryByUserIdAndArtId(user.id, dto.article_id))
                {

                    history.id_user = user.id;

                    await _mongoDbService.newHistoryAsync(history);

                    return true;
                }
                await _mongoDbService.updateHistoryDate(history);
                return true;
            }
            throw new UnauthorizedAccessException("Error: Token invalido");


        }
    }
}
