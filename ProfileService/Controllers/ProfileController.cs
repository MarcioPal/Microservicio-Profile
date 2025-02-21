using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProfileService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProfileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public ProfileController(MongoDbService _mongoDbService) { 
            this._mongoDbService = _mongoDbService;
        }

        // GET: api/<ProfileController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProfileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProfileController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Profile profile)
        {
            try
            {
                await _mongoDbService.newProfileAsync(profile);
                return Ok("El perfil fue registrado corrctamente");
            }
            catch (MongoWriteException ex) { 
                return BadRequest(ex.Message);
                
            }
        }

        // PUT api/<ProfileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProfileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
