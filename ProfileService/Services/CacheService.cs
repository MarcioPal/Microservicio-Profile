namespace ProfileService.Services;
using Microsoft.Extensions.Caching.Memory;
using ProfileService.Models;

public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetCacheValue(string key, User value)
    {
        // Configuración de la expiración
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(15)); // Expira si no se usa en 5 minutos

        _cache.Set(key, value, cacheEntryOptions);
    }

    public User GetCacheValue(string key)
    {
        return _cache.TryGetValue(key, out User value) ? value : null;
    }

    public void RemoveCacheValue(string key)
    {
        _cache.Remove(key);
    }
}

