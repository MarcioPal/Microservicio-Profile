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
                return Ok(await _historyService.Get(token));

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }

            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });

            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DtoHistory dtoHistory, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                return Ok(await _historyService.Save(token, dtoHistory));

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });

            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                await _historyService.Delete(token);
                return Ok(new { Message = "Se ha borrado el historial de articulos vistos correctamente"});
                
            }

            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });

            }
        }
    }
}
