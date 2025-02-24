using MongoDB.Bson.IO;
using System.Text;
using System;
using ProfileService.Models;
using System.Text.Json;



namespace ProfileService.Services
{
    public class IndireccionAuthService
    {
        private readonly CacheService _cache;
        public IndireccionAuthService(CacheService cache) { 
            _cache = cache;
        }

        public async Task<User> getUser(string token) {

            User? user = _cache.GetCacheValue(token);
            if(user == null)
            {
                string url = "http://localhost:3000/users/current";
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode) {
                    user = JsonSerializer.Deserialize<User>(result);
                    _cache.SetCacheValue(token, user);
                }
                //User usesr = MongoDB.Bson.IO.JsonConvert.DeserializeObject<User>(result);
            }
            return user;
        }
    }
}
