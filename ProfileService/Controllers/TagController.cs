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
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            this._tagService = tagService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
        {
            try
            {
                return Ok(await _tagService.Get(token, id));

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
        public async Task<IActionResult> Post([FromHeader(Name = "Authorization")] string token, [FromBody] Models.Tag tag)
        {
            try
            {
                await _tagService.Save(token, tag);
                return Ok(new { Message = "El Tag se ha registrado correctamente" });

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

        [HttpPut("{id}/articles/add")]
        public async Task<IActionResult> Put([FromHeader(Name = "Authorization")] string token, [FromBody] string article_id, [FromRoute] string id)
        {
            try
            {
                await _tagService.add_article(token, id, article_id);
                return Ok(new { Message = "El articulo se ha añadido correctamente" });
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

        [HttpPut("{id}/articles/remove")]
        public async Task<IActionResult> Remove([FromHeader(Name = "Authorization")] string token, [FromBody] String article_id, [FromRoute] string id)
        {
            try
            {
                await _tagService.delete_article(token, id, article_id);
                return Ok(new { Message = "El articulo ha sido removido correctamente" });
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Name([FromHeader(Name = "Authorization")] string token,  [FromRoute] string id, [FromBody] string name)
        {
            try
            {
                await _tagService.Name(token, id, name);
                return Ok("El nombre se ha actualizado correctamente");

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
