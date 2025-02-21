using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProfileService.Models;
using System.Xml.Linq;

public class MongoDbService
{
    private readonly IMongoCollection<Profile> _profilesCollection;

    public MongoDbService(IOptions<MongoDbSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _profilesCollection = database.GetCollection<Profile>("profiles");
    }

    public async Task<List<Profile>> getProfilesAsync() =>
        await _profilesCollection.Find(_ => true).ToListAsync();

    public async Task<Profile> getProfileByUserId(string userId) =>
        await _profilesCollection.Find(profile => profile.userId == userId).FirstOrDefaultAsync();

    public async Task newProfileAsync(Profile profile) =>
        await _profilesCollection.InsertOneAsync(profile);

    public class MongoDbSettings
    {
        public string DatabaseName { get; set;}
        public string ConnectionString { get; set;}
    }

}

