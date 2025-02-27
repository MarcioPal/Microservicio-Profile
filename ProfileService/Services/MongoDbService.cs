using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProfileService.Models;
using System.Xml.Linq;

public class MongoDbService
{
    private readonly IMongoCollection<Profile> _profilesCollection;
    private readonly IMongoCollection<History> _historyCollection;
    private readonly IMongoCollection<ProfileService.Models.Tag> _tagsCollection;

    public MongoDbService(IOptions<MongoDbSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _profilesCollection = database.GetCollection<Profile>("profiles");
        _historyCollection = database.GetCollection<History>("history");
        _tagsCollection = database.GetCollection<ProfileService.Models.Tag>("tags");
    }

    public async Task<List<Profile>> getProfilesAsync() =>
        await _profilesCollection.Find(_ => true).ToListAsync();

    public async Task<Profile> getProfileByUserId(string userId) =>
        await _profilesCollection.Find(profile => profile.userId == userId).FirstOrDefaultAsync();

    public async Task<bool> existsProfileByUserId(string userId) =>
    await _profilesCollection.Find(profile => profile.userId == userId).AnyAsync();

    public async Task newProfileAsync(Profile profile) =>
        await _profilesCollection.InsertOneAsync(profile);

    public async Task updateProfile(Profile profile)
    {
        var filter = Builders<Profile>.Filter.Eq(p => p.id, profile.id);
        var name = Builders<Profile>.Update.Set(p => p.name, profile.name);
        var lastname = Builders<Profile>.Update.Set(p => p.lastname, profile.lastname);
        var birthdate = Builders<Profile>.Update.Set(p => p.birthdate, profile.birthdate);
        var tag = Builders<Profile>.Update.Set(p => p.tag_id, profile.tag_id);
        var address = Builders<Profile>.Update.Set(p => p.address, profile.address);
        var imageId = Builders<Profile>.Update.Set(p => p.imageId, profile.imageId);
        var phone = Builders<Profile>.Update.Set(p => p.phone, profile.phone);

        await _profilesCollection.UpdateOneAsync(filter, name);
        await _profilesCollection.UpdateOneAsync(filter, lastname);
        await _profilesCollection.UpdateOneAsync(filter, birthdate);
        await _profilesCollection.UpdateOneAsync(filter, tag);
        await _profilesCollection.UpdateOneAsync(filter, address);
        await _profilesCollection.UpdateOneAsync(filter, imageId);
        await _profilesCollection.UpdateOneAsync(filter, phone);

    }

    //History of viewed articles
    public async Task<bool> existsHistoryByUserIdAndArtId(string userId, string article_id) =>
    await _historyCollection.Find(history => history.id_user == userId && history.article_id == article_id).AnyAsync();

    public async Task newHistoryAsync(History history) =>
    await _historyCollection.InsertOneAsync(history);

    public async Task<List<History>> getHistoryAsync(string userId) =>
    await _historyCollection.Find(history => history.id_user == userId).ToListAsync();

    public async Task updateHistoryDate(History history)
    {
        // Crear el filtro con los parámetros id_user y article_id
        var filter = Builders<History>.Filter.And(
            Builders<History>.Filter.Eq(h => h.id_user, history.id_user),
            Builders<History>.Filter.Eq(h => h.article_id, history.article_id)
        );

        // Asegurarse de que la fecha esté correctamente serializada
        var update = Builders<History>.Update.Set(h => h.date, history.date);

        // Realizar la actualización
        await _historyCollection.UpdateOneAsync(filter, update);
    }

    public async Task deleteAllHistoryByUser(string user_id)
    {
        var filter = Builders<History>.Filter.Eq(h => h.id_user,user_id);

        await _historyCollection.DeleteManyAsync(filter);
    }

    //Tags
    public async Task<ProfileService.Models.Tag> getTagAsync(string id) =>
            await _tagsCollection.Find(tag => tag.id == id).FirstOrDefaultAsync();

    public async Task newTagAsync(ProfileService.Models.Tag tag) =>
            await _tagsCollection.InsertOneAsync(tag);

    public async Task updateArticlesTag(ProfileService.Models.Tag tag)
    {
        var filter = Builders<ProfileService.Models.Tag>.Filter.And(
            Builders<ProfileService.Models.Tag>.Filter.Eq(t => t.id, tag.id));

        var update = Builders<ProfileService.Models.Tag>.Update.Set(t => t.articles, tag.articles);

        await _tagsCollection.UpdateOneAsync(filter, update);
    }

    public async Task updateTagName(ProfileService.Models.Tag tag)
    {
        var filter = Builders<ProfileService.Models.Tag>.Filter.And(
            Builders<ProfileService.Models.Tag>.Filter.Eq(t => t.id, tag.id));

        var update = Builders<ProfileService.Models.Tag>.Update.Set(t => t.name, tag.name);

        await _tagsCollection.UpdateOneAsync(filter, update);
    }


    public class MongoDbSettings
    {
        public string DatabaseName { get; set;}
        public string ConnectionString { get; set;}
    }

}

