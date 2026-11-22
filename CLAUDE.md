# EMagaza (LUNE)

Giyim e-ticaret öğrenme projesi. .NET 9, ASP.NET Core MVC, MongoDB, RabbitMQ (MassTransit), Docker, xUnit.
Kullanıcı teknolojileri öğrenmek için çalışıyor: değişiklik yaparken **neden** o şekilde yapıldığını kısa açıkla.

## Komutlar

```powershell
dotnet build                       # çözümü derle
dotnet test                        # 139 test; gerçek MongoDB/RabbitMQ gerekmez
dotnet test tests/EMagaza.Tests --filter "FullyQualifiedName~CouponTests"
docker compose up -d --build       # web http://localhost:8080
docker compose run --rm mongo-seed # örnek veriyi tekrar yükle (koleksiyon boşsa)
```

Portlar: web 8080 · Mongo 27018 · mongo-express 8081 · RabbitMQ 5673 (panel 15673, guest/guest).
Yerelde çalışan `EMagaza.Web`/`EMagaza.Worker` süreçleri derlemede DLL kilitler; önce onları kapat.

## Mimari (bağımlılık yönü: Web/Worker/Infrastructure → Application → Domain)

- `Domain`: entity'ler ve iş kuralları. Hiçbir pakete bağlı değil.
- `Application`: servisler (`CheckoutService` vb.) ve **arayüzler** (`IProductRepository`, `IPaymentGateway`). MongoDB/RabbitMQ'yu bilmez.
- `Infrastructure`: MongoDB repository'leri, migration'lar, MassTransit yayıncısı, ödeme simülasyonu.
- `Web`: MVC controller/view. `Worker`: kuyruktan bildirim ve e-posta kaydı üretir.
- `tests/EMagaza.Tests`: xUnit + elle yazılmış sahteler (`Fakes/`). Mock kütüphanesi yok.

## Kurallar

- Bir dosyada bir sınıf. Somut sınıflar sadece `Extensions/*ServiceCollectionExtensions.cs` içinde bağlanır.
- Entity'lerde `private set` + private parametresiz ctor + doğrulayan public ctor. Hatalar `DomainException`.
- Mongo: okuma LINQ (`AsQueryable`), yazma `Builders` ile (stok ve kupon atomik). `Id` eşlemesi `MongoConventions` içinde, sınıf başına yazılmaz.
- Şema/indeks değişikliği = yeni `IMigration` sınıfı (`M004...`). Uygulananlar `migrations` koleksiyonunda.
- Örnek veri kodda değil `db/seed/*.json` içinde. Ürün ve kupon eklemek için JSON'a ekle.
- Ödeme simülasyondur: `4242…` onaylanır, `…0002` reddedilir. Sadece son 4 hane saklanır.
- Yorum satırı az olsun; kod kendini anlatsın.
- Yeni davranışa test yaz (Arrange-Act-Assert). Veritabanı erişimi olan kodu sahte repository ile test et.

## Dikkat

- Silme/güncelleme yapan deneme betiklerinde kayıt id'sini tahmin etme; önce hedefi doğrula.
- `docs/` ders notlarıdır. Mimari değişince `docs/09-proje-yapisi.md` güncellenmeli. `docs/02-ef-core.md` ve `eski-ef-surumu/` geçmiştir: güncel kod EF kullanmaz.
- Kullanıcı istemeden `git commit` veya `git push` yapma.
