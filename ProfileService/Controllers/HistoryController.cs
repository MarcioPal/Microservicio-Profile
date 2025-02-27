using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProfileService.DTO;
using ProfileService.Models;
using ProfileService.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProfileService.Controllers
{
    [Route("v1/profile/history")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IndireccionAuthService _indireccionAuthService;
        private readonly IMapper _mapper;

        public HistoryController(MongoDbService _mongoDbService, IndireccionAuthService indireccionAuthService, IMapper mapper)
        {
            this._mongoDbService = _mongoDbService;
            this._indireccionAuthService = indireccionAuthService;
            this._mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string token)
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
                    List<History> lista = await _mongoDbService.getHistoryAsync(user.id);
                    return Ok(lista);
                }
                return BadRequest("Error: Token invalido");

            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DtoHistory dtoHistory, [FromHeader(Name = "Authorization")] string token)
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
                    History history = _mapper.Map<History>(dtoHistory);
                    if (!await _mongoDbService.existsHistoryByUserIdAndArtId(user.id, dtoHistory.article_id))
                    {

                        history.id_user = user.id;

                        await _mongoDbService.newHistoryAsync(history);

                        return Ok("El articulo se ha añadido correctamente al historial de visualizaciones");
                    }
                    await _mongoDbService.updateHistoryDate(history);
                    return Ok("Se ha actualizado el historial de articulos vistos");
                }
                return BadRequest("Error: Token invalido");

            }
            catch (MongoWriteException ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("{user_id}")]
        public async Task<IActionResult> Delete([FromRoute] string user_id, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                if (token is null)
                {
                    return Unauthorized(new { Message = "Error: No se encuentra logueado" });
                }

                User user = await _indireccionAuthService.getUser(token);
                if (user is not null)
                {
                    await _mongoDbService.deleteAllHistoryByUser(user_id);
                    return Ok(new { Message = "Se ha borrado el historial de articulos vistos correctamente"});
                }
                return BadRequest(new { Message = "Error: Token invalido" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            } 
        }
    }
}
