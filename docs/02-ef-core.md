# 02 · EF Core (Entity Framework Core)

> Not: Güncel proje MongoDB kullanır; EF Core ilişkisel veritabanları (SQL Server, SQLite) içindir ve çalışan koddan çıkarıldı. Eski EF'li sürüm `eski-ef-surumu/` klasöründe. Bu ders EF mantığını anlatır; Mongo karşılıkları için `07-mongodb.md` ve `09-proje-yapisi.md` tablosuna bak.

EF Core = **ORM**: C# sınıfları ↔ veritabanı tabloları. SQL yazmadan LINQ ile sorgularsın, o SQL'e çevirir.

## Parçalar ve dosyalar
| Kavram | Bu projede |
|---|---|
| Entity | `Domain/Entities/Product.cs`, `Order.cs` |
| DbContext (DB'ye açılan kapı + Unit of Work) | `Infrastructure/Persistence/StoreDbContext.cs` |
| DbSet<T> (tablo) | `Products`, `Orders` |
| Fluent API (kurallar) | `OnModelCreating` |
| Repository (sorgular) | `Persistence/Repositories.cs` |
| Migration (şema geçmişi) | `Persistence/Migrations/*InitialCreate*` |
| Seed (örnek veri) | `Persistence/DataSeeder.cs` |

## Neden SQLite?
Kurulum yok, tek dosya (`App_Data/store.db`). Sağlayıcıyı değiştirmek tek satır: `UseSqlite` → `UseSqlServer`. Mantık aynı kalır.

## Adım adım
**1. Paket:** `dotnet add src/EMagaza.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.0`
(Web'e `Microsoft.EntityFrameworkCore.Design` — `dotnet ef` aracı başlangıç projesinden okur.)

**2. DbContext** — `DbContextOptions` constructor'dan gelir, `UseSqlite` DI'da verilir:
```csharp
services.AddDbContext<StoreDbContext>(o => o.UseSqlite(connectionString)); // Scoped kayıt
```

**3. Fluent API** — `Price` için `HasPrecision(18,2)` (para birimi, yuvarlama hatası olmasın); `Order.Total` hesaplanan özellik → `Ignore` (kolon olmasın); `Order 1—N OrderItem` ilişkisi `HasMany/WithOne`.

**4. Migration** (şemanın Git'i):
```powershell
dotnet tool install -g dotnet-ef                       # bir kez
dotnet ef migrations add InitialCreate -p src/EMagaza.Infrastructure -s src/EMagaza.Web -o Persistence/Migrations
dotnet ef database update -p src/EMagaza.Infrastructure -s src/EMagaza.Web   # elle uygulamak istersen
```
`-p` = migration'ın yazılacağı proje (DbContext'in olduğu), `-s` = startup (config/DI'nin olduğu). Karıştırılan #1 hata budur.
Biz uygulamada `db.Database.Migrate()` çağırıyoruz (Program.cs): açılışta bekleyen migration'lar otomatik uygulanır.
> Üretimde çok sunuculu ortamda otomatik Migrate riskli (iki sunucu yarışır); orada migration bundle / ayrı adım kullanılır.

## Kritik davranışlar (mülakat altın madeni)
- **Change Tracking:** Veritabanından çektiğin entity izlenir. `p.DecreaseStock(2)` yaptıktan sonra sadece `SaveChanges()` çağırırsın, EF farkı bulup `UPDATE` üretir. `CheckoutService` tam bunu kullanır.
- **SaveChanges = tek transaction.** Sipariş + stok düşümü ya birlikte yazılır ya hiç. Yetersiz stokta exception, `SaveChanges`'e hiç gelinmez → test: `PlaceOrder_InsufficientStock_ThrowsAndSavesNothing`.
- **Include:** İlişkili veri otomatik gelmez (lazy loading kapalı). `GetAsync`'te `.Include(o => o.Items)`. Unutursan `Items` boş döner — sessiz hata.
- **N+1:** Döngüde her kayıt için ayrı sorgu atmak. Çözüm `Include` veya tek sorguda `Where(ids.Contains)` (`GetByIdsAsync` böyle).
- **IQueryable vs List:** `SearchAsync`'te `Where` zincirleri SQL'e çevrilir; `ToListAsync()` çağrılana kadar sorgu çalışmaz (deferred execution).
- **AsNoTracking:** Sadece okuyorsan hızlandırır (liste sayfaları için eklenebilir).

## Gerçek SQL'i görmek
Uygulama loglarında EF sorguları görünür (Development):
```sql
SELECT "p"."Id", "p"."Category", ... FROM "Products" AS "p" ORDER BY "p"."Name"
```
## Sık hatalar
- `-p/-s` karıştırmak → "Unable to create DbContext".
- Model değişti ama migration eklenmedi → "no such column".
- Decimal'e precision vermemek → SQLite'ta uyarı / SQL Server'da kırpma.
## Mülakat soruları
Code-first vs DB-first? Migration nedir? Tracking nedir, ne zaman kapatılır? N+1 nedir? `SaveChanges` transaction mı? Lazy/Eager/Explicit loading?
