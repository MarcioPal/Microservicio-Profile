using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ProfileService.Models;
using ProfileService.DTO;
using ProfileService.Services;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProfileService.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IndireccionAuthService _indireccionAuthService;
        private readonly IMapper _mapper;

        public ProfileController(MongoDbService _mongoDbService, IndireccionAuthService indireccionAuthService, IMapper mapper) {
            this._mongoDbService = _mongoDbService;
            this._indireccionAuthService = indireccionAuthService;
            this._mapper = mapper;
        }

        // GET: api/<ProfileController>
        [HttpGet("current")]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string? token)
        {
            try
            {
                if (token is null) {
                    return Unauthorized("Error: No se encuentra logueado o no tiene los permisos requeridos");
                }
                User user = await _indireccionAuthService.getUser(token);
                if (user is not null)
                {
                    if (await _mongoDbService.existsProfileByUserId(user.id))
                    {

                        return Ok(await _mongoDbService.getProfileByUserId(user.id));
                    }
                    return NotFound("Error: No existe un perfil creado para el usuario logueado");
                }
                return BadRequest("Error: Token invalido");
            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }

        // GET api/<ProfileController>/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute(Name = "userId")] string userId, [FromHeader(Name = "Authorization")] string? token)
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
                        if (await _mongoDbService.existsProfileByUserId(userId))
                        {

                            return Ok(await _mongoDbService.getProfileByUserId(user.id));
                        }
                        return NotFound("Error: No se encuentra el perfil requerido");
                    }
                    return Unauthorized("Error: No tiene permiso de admin");
                }
                return BadRequest("Error: Token invalido");
            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }

        // POST api/<ProfileController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DtoProfile DtoProfile, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
                }

                User user = await _indireccionAuthService.getUser(token);

                if (await _mongoDbService.existsProfileByUserId(user.id)) {
                    return BadRequest("Ya existe un perfil para el usuario");
                }
                Models.Profile profile = _mapper.Map<Models.Profile>(DtoProfile);
                profile.userId = user.id;
                await _mongoDbService.newProfileAsync(profile);
                return Ok("El perfil fue registrado correctamente");
            }
            catch (MongoWriteException ex) { 
                return BadRequest(ex.Message);
                
            }
        }

        // PUT api/<ProfileController>/5
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Models.Profile profilePut,[FromHeader(Name = "Authorization")] string? token)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized("Error: No se encuentra logueado");
                }
                User user = await _indireccionAuthService.getUser(token);
                if (user is not null) {
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
                        return Ok("El perfil fue actualizado correctamente");
                    }
                }
                return BadRequest("Error: Token invalido");

            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }


        [HttpGet("suggestions")]
        public async Task<IActionResult> getAsync([FromHeader(Name = "Authorization")] string? token)
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
                    Models.Profile profile = await _mongoDbService.getProfileByUserId(user.id);
                    Models.Tag tag = await _mongoDbService.getTagAsync(profile.tag_id);
                    List<History> vistos = await _mongoDbService.getHistoryAsync(user.id);
                    List<string> sugerencias= new List<string>();   

                    foreach(string article in tag.articles)
                    {
                        bool visto = false;
                        foreach (History art in vistos) {
                            if (article == art.article_id) {
                                visto = true;
                            }
                        }
                        if(!visto) {
                            sugerencias.Add(article);
                        }

                    }
                    return Ok(sugerencias);
                }
                return BadRequest(new { Error = "Token invalido" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
            
        }
            // DELETE api/<ProfileController>/5
            [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
