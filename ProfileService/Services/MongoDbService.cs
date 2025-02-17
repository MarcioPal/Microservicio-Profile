using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProfileService.Models;

public class MongoDbService
{
    private readonly IMongoCollection<Profile> _profilesCollection;

    public MongoDbService(IOptions<MongoDbSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _profilesCollection = database.GetCollection<Profile>("profiles");
    }

    public async Task<List<Profile>> ObtenerUsuariosAsync() =>
        await _profilesCollection.Find(_ => true).ToListAsync();

    public async Task InsertarUsuarioAsync(Profile usuario) =>
        await _profilesCollection.InsertOneAsync(usuario);

    public class MongoDbSettings
    {
        public string DatabaseName { get; set;}
        public string ConnectionString { get; set;}
    }

}

