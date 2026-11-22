# 07 · MongoDB (ürün yorumları)

## Neden yorumlar MongoDB'de, ürünler SQL'de?
| | SQL (ürün, sipariş) | MongoDB (yorum) |
|---|---|---|
| Veri şekli | Tablo, sabit kolonlar | **Belge** (JSON benzeri), esnek |
| İlişki | Sipariş↔Kalem↔Ürün: JOIN ve tutarlılık şart | Yorum kendi başına bir bütün |
| Para/stok | Transaction şart | Kaybolsa dünya yıkılmaz |
Kural: **parayı ve stoğu ilişkisel veritabanında tut; esnek, çok okunan, bağımsız içeriği belge veritabanında tut.** İkisini birden kullanmaya *polyglot persistence* denir.
Dürüst not: 12 ürünlük mağazada yorumlar SQLite'ta da dururdu. Burada amaç Mongo'yu bir işte görmek.

## Kavram eşlemesi
| SQL | MongoDB |
|---|---|
| Database | Database (`emagaza`) |
| Table | **Collection** (`reviews`) |
| Row | **Document** |
| Primary key `Id` | `_id` (**ObjectId**) |
| Index | Index (`ProductId`) |
Belge şeması yok: aynı koleksiyona farklı alanlı belge girebilir. Bu esneklik, hata yakalamayı **sana** bırakır (SQL'de veritabanı yapar).

## Gerçek belge (doğrulandı, mongosh çıktısı)
```js
{ _id: ObjectId('6b01794d91587e4236d9b10d'), ProductId: 1, Author: 'Ayşe', Rating: 5,
  Comment: 'Ses kalitesi harika...', CreatedAt: ISODate('2026-11-21T10:58:21.541Z') }
```
`docker exec emagaza-mongo-1 mongosh emagaza --eval "db.reviews.find()"` ile sen de görebilirsin.

## Kodda neresi
| Dosya | Görev |
|---|---|
| `Domain/Entities/Review.cs` | Sınıf + `Review.Create` (puan 1-5, boş alan kontrolü). Mongo'yu BİLMEZ |
| `Application/Abstractions/Reviews.cs` | `IReviewRepository` arayüzü |
| `Infrastructure/Mongo/MongoReviewRepository.cs` | `MongoDB.Driver` ile ekle/oku, indeks |
| `Infrastructure/DependencyInjection.cs` | `IMongoClient` Singleton kaydı |
| `Web/Controllers/ReviewsController.cs` | Yorum POST'u |
| `Web/Views/Shop/Detail.cshtml` | Yorum listesi + ortalama puan + form |

## Önemli karar noktaları
**1. `[BsonId]` attribute'u yerine `BsonClassMap`.** Domain'e `MongoDB.Bson` paketini sokmamak için eşleme Infrastructure'da yapılır (`MapIdMember(...).SetSerializer(new StringSerializer(BsonType.ObjectId))`). Sonuç: `Review.Id` C#'ta düz `string`, Mongo'da `ObjectId`.

**2. `IMongoClient` Singleton.** İçinde bağlantı havuzu var, thread-safe, pahalı. Her istekte yenisini açmak klasik hata.

**3. İki veritabanı arası ilişki.** `Review.ProductId` SQL'deki `Product.Id`'yi *mantıksal* gösterir. Veritabanları arası foreign key YOKTUR: ürün silinirse yorumlar öksüz kalır. Bu, polyglot'un bedeli.

**4. Zarif bozulma (gerçek bir hatayla öğrendim).** Mongo kapalıyken ürün sayfası ÖNCE 500 verdi. Sebep: indeks oluşturmayı repository'nin *constructor'ında* yapmıştım; bağımlılık çözülürken ağ çağrısı patladı ve benim `try/catch`'im çalışma şansı bulamadı. Çözüm: constructor'da ağ çağrısı yok; indeks açılışta `try/catch` içinde. Sonuç (ölçüldü): Mongo kapalı → HTTP 200 + "Yorumlar şu an yüklenemiyor". **Ders: constructor'da I/O yapma.**
`ServerSelectionTimeout = 2 sn`: varsayılan 30 sn, kapalı Mongo sayfayı yarım dakika bekletirdi.

## Komutlar
```powershell
docker compose up -d mongo
docker exec -it emagaza-mongo-1 mongosh emagaza
```
```js
db.reviews.find({ ProductId: 1 }).sort({ CreatedAt: -1 })
db.reviews.aggregate([{ $group: { _id: "$ProductId", ort: { $avg: "$Rating" }, adet: { $sum: 1 } } }])
db.reviews.getIndexes()
db.reviews.find({ ProductId: 1 }).explain("executionStats")   // IXSCAN mı COLLSCAN mı?
```
Uygulamada ortalamayı C#'ta hesapladım (`Average`). Çok yorum olunca doğrusu `aggregate` ile Mongo'ya hesaplatmak.

## Mülakat soruları
SQL vs NoSQL ne zaman? Collection/Document nedir? ObjectId nedir (zaman damgası içerir)? Index neden ve `explain` nasıl okunur? Embed mı reference mi (yorumu ürün belgesinin içine gömmek vs ayrı koleksiyon: sınırsız büyüyen veriyi gömme, 16 MB belge sınırı)? CAP teoremi? Mongo transaction destekler mi (evet, replica set'te)?
