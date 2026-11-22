# Proje yapısı ve MongoDB yazım stili

## Katmanlar ve klasörler

```
src/
  EMagaza.Domain           ← iş kuralları, hiçbir şeye bağımlı değil
    Entities/              Product, SizeStock, Order, OrderItem, User, Review, Notification, EmailLog (+ Entity tabanı)
    Enums/  Constants/  Exceptions/   OrderStatus, Roles, DomainException, NotFoundException
  EMagaza.Application      ← kullanım senaryoları; sadece arayüzleri bilir
    Interfaces/Repositories/   IProductRepository ...        (veri erişim sözleşmeleri)
    Interfaces/Infrastructure/ IPaymentGateway, IPasswordHasher, IOrderEventPublisher, IQueueMonitor
    Interfaces/Services/       ICheckoutService, ICartService, IAuthService ...
    Services/                  CheckoutService, CartService, AuthService ... (arayüzlerin gerçekleştirimi)
    Carts/ Models/ Options/ Exceptions/ Formatting/ Notifications/ Extensions/
  EMagaza.Infrastructure   ← dış dünya: MongoDB, RabbitMQ (MassTransit), ödeme, şifreleme
    Persistence/Mongo/         MongoDbContext, MongoConventions, MongoIndexInitializer
    Persistence/Repositories/  MongoRepositoryBase + her koleksiyon için bir repository
    Messaging/ Payments/ Security/ Extensions/
  EMagaza.Web              ← MVC: Controllers, Models (form/view modelleri), Services (ICartStore), Helpers, Views
  EMagaza.Worker           ← MassTransit tüketicisi (Consumers/OrderEventConsumer)
tests/EMagaza.Tests        ← Domain, Carts, Services, Infrastructure, Notifications + Fakes
db/seed/                   ← örnek veri (products.json) ve yükleme script'i (seed.sh)
```

Bağımlılık yönü: `Web/Worker → Application → Domain`, `Infrastructure → Application → Domain`.
Application, MongoDB'yi veya RabbitMQ'yu hiç görmez; sadece arayüzleri görür.

## SOLID nerede?

- **S** — Her sınıf tek iş: `StockReservation` stok ayırır, `CardValidator` kartı doğrular, `OrderMessageBuilder` metin üretir.
- **O** — Yeni ürün çizimi eklemek için `GarmentShapes` listesine bir metot eklemek yeterli; yeni ödeme sağlayıcısı için yeni bir `IPaymentGateway` sınıfı yazılır.
- **L** — `ProductRepository` ve test sahtesi `FakeProductRepository`, `IProductRepository` yerine birbirinin yerine geçebilir.
- **I** — Küçük arayüzler: `IEmailLogRepository` sadece e-posta kaydıyla ilgili metotlar taşır.
- **D** — Controller'lar `ICheckoutService`'e, servisler `IProductRepository`'ye bağlıdır; somut sınıflar sadece `Extensions/ServiceCollectionExtensions.cs` içinde bağlanır.

## MongoDB'de EF Core / LINQ karşılığı

| SQL / EF Core | MongoDB.Driver |
|---|---|
| `DbContext` | `MongoDbContext` (koleksiyonlara tek noktadan erişim) |
| `db.Products.Where(...).OrderBy(...).ToListAsync()` | `Collection.AsQueryable().Where(...).OrderBy(...).ToListAsync()` — aynı LINQ |
| `Add / SaveChanges` | `InsertOneAsync` |
| `UPDATE ... SET x = x - 1` | `UpdateOneAsync(filtre, Builders<T>.Update.Inc(...))` (atomik) |
| Kayıt yoksa ekle | `UpdateOneAsync(..., new UpdateOptions { IsUpsert = true })` |
| Migration | MongoDB'de yerleşik yok (şema esnek). `Persistence/Migrations/`: `IMigration` + `MigrationRunner`; uygulananlar `migrations` koleksiyonunda (EF'in `__EFMigrationsHistory`'si gibi). Yeni değişiklik = yeni `M003...` sınıfı |
| Seed | `db/seed/products.json` + `mongo-seed` servisi (koda gömülü veri yok) |

Okuma için LINQ, kısmi/atomik güncelleme için `Builders` kullanılır. `Id` eşlemesi sınıf başına yazılmaz:
`MongoConventions` içindeki tek kural paketi ("Id alanı _id'dir, string Id ObjectId saklanır, enum yazı saklanır") hepsini kapsar.

## Örnek veriyi yüklemek

`docker compose up` ile `mongo-seed` servisi çalışır; `products` koleksiyonu boşsa `db/seed/products.json` yüklenir, doluysa dokunmaz.
Elle: `docker compose run --rm mongo-seed`.

## E-ticaret özellikleri (2026-11-22)

| Özellik | Nerede |
|---|---|
| Kupon (yüzde/sabit, min. tutar, süre, kullanım limiti; atomik hak alma) | `Coupon`, `CouponService`, `CouponRepository`, `db/seed/coupons.json` |
| Favoriler | `Favorite`, `FavoriteService`, `FavoritesController` |
| Adres defteri + şifre değiştirme | `Address`, `AddressService`, `AuthService.ChangePasswordAsync`, `Account/Profile` |
| Sipariş iptali (stok iadesi, ödeme iadesi, kupon hakkı iadesi, bildirim) | `Order.Cancel`, `OrderManagementService.CancelAsync` |
| Ürün yönetimi (ekle/düzenle/sil, beden bazlı stok) | `ProductManagementService`, `Admin/Products`, `Admin/ProductEdit` |
| Sayfalama, ürün puan özeti | `PagedResult<T>`, `Product.RatingAverage`, `ReviewService` |
| Migration M003 | favori/kupon/adres indeksleri |

Test kuponları: `HOSGELDIN10` (%10), `YAZ200` (1.500 ₺ üzeri 200 ₺), `SON3` (3 kullanımlık %25).
