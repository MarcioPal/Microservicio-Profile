using AutoMapper;
using Newtonsoft.Json.Linq;
using ProfileService.DTO;
using ProfileService.Models;

namespace ProfileService.Services
{
    public class ProfileService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IndireccionAuthService _indireccionAuthService;
        private readonly IMapper _mapper;

        public ProfileService(MongoDbService _mongoDbService, IndireccionAuthService indireccionAuthService, IMapper mapper)
        {
            this._mongoDbService = _mongoDbService;
            this._indireccionAuthService = indireccionAuthService;
            this._mapper = mapper;
        }

        public async Task<Models.Profile> Get(string token)
        {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado o no tiene los permisos requeridos");
            }

            User user = await _indireccionAuthService.getUser(token);

            if (user is not null)
            {
                if (await _mongoDbService.existsProfileByUserId(user.id))
                {

                    return await _mongoDbService.getProfileByUserId(user.id);
                }
                throw new Exception("Error: No existe un perfil creado para el usuario logueado");
            }

            throw new UnauthorizedAccessException("Error: Token invalido");
        }

        public async Task<Models.Profile> Get(string token, string user_id)
        {
            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }
            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                if (user.permissions.Contains("admin"))
                {
                    if (await _mongoDbService.existsProfileByUserId(user_id))
                    {

                        return await _mongoDbService.getProfileByUserId(user.id);
                    }
                    throw new Exception("Error: No se encuentra el perfil requerido");
                }
                throw new UnauthorizedAccessException("Error: No tiene permiso de admin");
            }
            throw new UnauthorizedAccessException("Error: Token invalido");
        }

        public async void Save(string token, DtoProfile dtoProfile) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                if (await _mongoDbService.existsProfileByUserId(user.id))
                {
                    throw new Exception("Ya existe un perfil para el usuario");
                }
                Models.Profile profile = _mapper.Map<Models.Profile>(dtoProfile);
                profile.userId = user.id;
                await _mongoDbService.newProfileAsync(profile);
                return;
            }
            throw new UnauthorizedAccessException("Error: Token invalido");

        }

        public async void Update(string token, Models.Profile profilePut) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }
            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                if (await _mongoDbService.existsProfileByUserId(user.id))
                {
                    Models.Profile profile = await _mongoDbService.getProfileByUserId(user.id);
                    profile.tag_id = profilePut.tag_id;
                    profile.address = profilePut.address;
                    profile.phone = profilePut.phone;
                    profile.name = profilePut.name;
                    profile.birthdate = profilePut.birthdate;
                    profile.lastname = profilePut.lastname;
                    profile.imageId = profilePut.imageId;
                    await _mongoDbService.updateProfile(profile);
                }
            }
            throw new Exception("Error: Token invalido");
        }

        public async Task<List<string>> getSugerencias(string token) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }
            User user = await _indireccionAuthService.getUser(token);

            if (user is not null)
            {
                Models.Profile profile = await _mongoDbService.getProfileByUserId(user.id);
                Models.Tag tag = await _mongoDbService.getTagAsync(profile.tag_id);
                List<History> vistos = await _mongoDbService.getHistoryAsync(user.id);
                List<string> sugerencias = new List<string>();

                foreach (string article in tag.articles)
                {
                    bool visto = false;
                    foreach (History art in vistos)
                    {
                        if (article == art.article_id)
                        {
                            visto = true;
                        }
                    }
                    if (!visto)
                    {
                        sugerencias.Add(article);
                    }

                }
                return sugerencias;
            }
            throw new UnauthorizedAccessException("Token invalido");
        }

    }
}
