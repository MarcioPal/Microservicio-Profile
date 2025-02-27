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
        private readonly ProfileService.Services.ProfileService _profileService;
        
        public ProfileController(ProfileService.Services.ProfileService profileService) {
            this._profileService = profileService;
        }

        // GET: api/<ProfileController>
        [HttpGet("current")]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string? token)
        {
            try
            {
               return Ok(await _profileService.Get(token));
            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            catch (Exception ex)
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
              return Ok(await _profileService.Get(token, userId));
            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        // POST api/<ProfileController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DtoProfile dtoProfile, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                 _profileService.Save(token, dtoProfile);
                return Ok("El perfil fue registrado correctamente");

            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        // PUT api/<ProfileController>/5
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Models.Profile profilePut,[FromHeader(Name = "Authorization")] string? token)
        {
            try
            {
                _profileService.Update(token, profilePut);
                return Ok("El perfil fue actualizado correctamente");
            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }


        [HttpGet("suggestions")]
        public async Task<IActionResult> Sugerencias([FromHeader(Name = "Authorization")] string? token)
        {
            try
            {
                return Ok(_profileService.getSugerencias(token));
            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
            
        }

    }
}
