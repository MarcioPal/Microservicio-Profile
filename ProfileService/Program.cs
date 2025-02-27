using Microsoft.Extensions.Hosting;
using ProfileService.Controllers;
using ProfileService.Services;
using ProfileService.Services.Cache;
using ProfileService.Services.Rabbit;
using static MongoDbService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cargar configuración desde appsettings.json
builder.Services.Configure<MongoDbSettings>(
builder.Configuration.GetSection("MongoDB"));
builder.Services.AddMemoryCache();

// Registrar el servicio de MongoDB
builder.Services.AddSingleton<MongoDbService>(); 
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<IndireccionAuthService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton<HistoryService>();
builder.Services.AddSingleton<ProfileService.Services.ProfileService>();
builder.Services.AddSingleton<ProfileService.Services.TagService>();
builder.Services.AddScoped<HistoryController>();
builder.Services.AddHostedService<RabbitMqListenerService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
