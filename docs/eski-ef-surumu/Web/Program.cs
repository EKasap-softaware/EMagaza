using EMagaza.Infrastructure;
using EMagaza.Infrastructure.Messaging;
using EMagaza.Infrastructure.Mongo;
using EMagaza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(o => { o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });

// Bağlantı dizesi appsettings.json'dan (veya ortam değişkeni ConnectionStrings__Store) gelir.
var cfg = builder.Configuration;
builder.Services.AddInfrastructure(
    cfg.GetConnectionString("Store") ?? "Data Source=App_Data/store.db",
    new MongoSettings(cfg["Mongo:ConnectionString"]!, cfg["Mongo:Database"] ?? "emagaza"),
    new RabbitMqSettings(cfg["RabbitMq:Host"] ?? "localhost", int.Parse(cfg["RabbitMq:Port"] ?? "5672")));

var app = builder.Build();

// Açılışta: bekleyen migration'ları uygula + örnek ürünleri ekle.
using (var scope = app.Services.CreateScope())
{
    Directory.CreateDirectory("App_Data");
    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    db.Database.Migrate();
    DataSeeder.Seed(db);

    // Mongo indeksi: Mongo kapalıysa uygulama yine açılsın (sadece uyarı).
    try { MongoReviewRepository.EnsureIndexes(scope.ServiceProvider.GetRequiredService<MongoDB.Driver.IMongoClient>(),
                                              scope.ServiceProvider.GetRequiredService<MongoSettings>()); }
    catch (Exception ex) { app.Logger.LogWarning("Mongo indeksi oluşturulamadı: {Msg}", ex.Message); }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shop/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(name: "default", pattern: "{controller=Shop}/{action=Index}/{id?}");

app.Run();

public partial class Program { } // integration test'ler için
