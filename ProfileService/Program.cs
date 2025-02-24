using ProfileService.Services;
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
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<IndireccionAuthService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

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
