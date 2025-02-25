using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProfileService.Models;
using ProfileService.Services;

namespace ProfileService.Controllers
{
    [Route("v1/profile/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IndireccionAuthService _indireccionAuthService;
        private readonly IMapper _mapper;

        public TagController(MongoDbService _mongoDbService, IndireccionAuthService indireccionAuthService, IMapper mapper)
        {
            this._mongoDbService = _mongoDbService;
            this._indireccionAuthService = indireccionAuthService;
            this._mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
                }

                User user = await _indireccionAuthService.getUser(token);
                if (user is not null)
                {
                    if (user.permissions.Contains("admin"))
                    {
                        Models.Tag tag = await _mongoDbService.getTagAsync(id);
                        return Ok(tag);
                    }
                    return Unauthorized("Error: No tiene permiso de admin");
                }
                return BadRequest("Error: Token invalido");

            }
            catch (MongoException ex)
            {
                return BadRequest(ex.Message);

            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromHeader(Name = "Authorization")] string token, [FromBody] Models.Tag tag)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
                }

                User user = await _indireccionAuthService.getUser(token);
                if (user is not null)
                {
                    if (user.permissions.Contains("admin"))
                    {
                        await _mongoDbService.newTagAsync(tag);
                        return Ok($"id_tag: {tag.id}");
                    }
                    return Unauthorized("Error: No tiene permiso de admin");
                }
                return BadRequest("Error: Token invalido");

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPut("{id}/articles/add")]
        public async Task<IActionResult> Put([FromHeader(Name = "Authorization")] string token, [FromBody] string article_id, [FromRoute] string id)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
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

                            var response = new { Message = "El articulo se ha añadido correctamente" };

                            return Ok(response);
                        }
                    }
                    return BadRequest(new { Message = "Error: No tiene permisos de admin" });
                }
                return BadRequest(new { Message = "Token invalido" });
            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpPut("{id}/articles/remove")]
        public async Task<IActionResult> Remove([FromHeader(Name = "Authorization")] string token, [FromBody] String article_id, [FromRoute] string id)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
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
                            return Ok(new { Message = "El articulo ha sido removido correctamente" });
                        }
                        return BadRequest(new { Error = "El articulo especificado no existe en la lista" });
                    }
                    return Unauthorized(new { Error = "No tiene permisos de admin" });
                }
                return BadRequest(new { Error = "Token invalido" });
            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Name([FromHeader(Name = "Authorization")] string token,  [FromRoute] string id, [FromBody] string name)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
                }

                User user = await _indireccionAuthService.getUser(token);
                if (user is not null)
                {
                    if (user.permissions.Contains("admin"))
                    {
                        Models.Tag tag = await _mongoDbService.getTagAsync(id);
                        tag.name = name;
                        await _mongoDbService.updateTagName(tag);
                        return Ok(new { Message = "El nombre se ha actualizado correctamente" });
                    }
                    return Unauthorized(new { Error = "No tiene permisos de admin" });
                }
                return BadRequest(new { Error = "Token invalido" });

            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }
    }
}
