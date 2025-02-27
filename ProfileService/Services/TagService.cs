using AutoMapper;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ProfileService.Models;
using System.Xml.Linq;
using Tag = ProfileService.Models.Tag;

namespace ProfileService.Services
{
    public class TagService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IndireccionAuthService _indireccionAuthService;
        private readonly IMapper _mapper;

        public TagService(MongoDbService _mongoDbService, IndireccionAuthService indireccionAuthService, IMapper mapper)
        {
            this._mongoDbService = _mongoDbService;
            this._indireccionAuthService = indireccionAuthService;
            this._mapper = mapper;
        }


        public async Task<Tag> Get(string token, string id) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                if (user.permissions.Contains("admin"))
                {
                    Models.Tag tag = await _mongoDbService.getTagAsync(id);
                    return tag;
                }
                throw new UnauthorizedAccessException("Error: No tiene permiso de admin");
            }
            throw new Exception("Error: Token invalido");
        }

        public async void Save(string token, Tag tag)
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
                    await _mongoDbService.newTagAsync(tag);
                    return;
                }
                throw new UnauthorizedAccessException("Error: No tiene permiso de admin");
            }
            throw new UnauthorizedAccessException("Error: Token invalido");
        }

        public async void Name(string token, string id, string name)
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
                    Models.Tag tag = await _mongoDbService.getTagAsync(id);
                    tag.name = name;
                    await _mongoDbService.updateTagName(tag);
                    return;
                }
                throw new UnauthorizedAccessException("No tiene permisos de admin");
            }
            throw new UnauthorizedAccessException("Token invalido");
        }



        public async void add_article(string token, string id,string article_id) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);

            if (user is not null)
            {
                if (user.permissions.Contains("admin"))
                {
                    Models.Tag tag = await _mongoDbService.getTagAsync(id);
                    if (!tag.articles.Contains(article_id))
                    {
                        tag.articles.Add(article_id);

                        await _mongoDbService.updateArticlesTag(tag);
                        return;
                    }
                }
                throw new UnauthorizedAccessException("Error: No tiene permisos de admin");
            }
            throw new UnauthorizedAccessException("Token invalido");
        }

        public async void delete_article(string token, string id, string article_id) {

            if (token is null)
            {
                throw new UnauthorizedAccessException("Error: No se encuentra logueado");
            }

            User user = await _indireccionAuthService.getUser(token);
            if (user is not null)
            {
                if (user.permissions.Contains("admin"))
                {
                    Models.Tag tag = await _mongoDbService.getTagAsync(id);
                    if (tag.articles.Contains(article_id))
                    {
                        tag.articles.Remove(article_id);
                        return;
                    }
                    throw new Exception("El articulo especificado no existe en la lista");
                }
                throw new UnauthorizedAccessException("No tiene permisos de admin");
            }
            throw new Exception("Token invalido");
        }
    }
}
