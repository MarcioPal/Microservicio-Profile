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
        private readonly HistoryService _historyService;


        public HistoryController(HistoryService historyService)
        {
            this._historyService = historyService;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                return Ok(_historyService.Get(token));

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


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DtoHistory dtoHistory, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                return Ok(_historyService.Save(token, dtoHistory));

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

        [HttpDelete("{user_id}")]
        public async Task<IActionResult> Delete([FromRoute] string user_id, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                _historyService.Delete(token, user_id);
                return Ok(new { Message = "Se ha borrado el historial de articulos vistos correctamente"});
                
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
