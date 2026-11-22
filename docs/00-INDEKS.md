# EMağaza – Öğrenme Rehberi

Bu proje, mülakatta sorulan teknolojileri **tek bir gerçek uygulama üzerinden** öğrenmek için yazıldı.
Her dosya: *ne yaptım → hangi kodu yazdım → neden → hangi komut → sık hatalar → mülakat soruları*.

## Hızlı çalıştırma
```powershell
dotnet run --project src/EMagaza.Web --urls http://localhost:5020   # http://localhost:5020
dotnet test                                                          # 16 test
docker compose up -d --build                                         # tüm sistem: http://localhost:8080 (+ mongo, rabbitmq, worker)
# Yerelde çalıştırmak için önce: docker compose up -d mongo rabbitmq ; sonra Web VE Worker'ı ayrı terminallerde dotnet run
```

## ⚠ Sürüm notu
Proje önce SQLite + EF Core ile yazıldı, sonra **tamamen MongoDB'ye taşındı** (kullanıcı kararı) ve kayıt/giriş + yönetim paneli eklendi.
- Eski EF/SQLite kodu silinmedi: `docs/eski-ef-surumu/` (karşılaştırma için).
- `02-ef-core.md`, `07-mongodb.md`, `08-rabbitmq.md` ESKİ sürümü anlatıyor; sohbetteki anlatım turunda bu dosyalar yeni koda göre yeniden yazılacak. Şimdilik **koda ve sohbete güven**.

## Çalıştırma (güncel)
```powershell
docker compose up -d --build     # web + worker + mongo + rabbitmq + mongo-express
```
| Ne | Adres |
|---|---|
| Mağaza | http://localhost:8080 |
| Yönetim paneli | http://localhost:8080/Admin (admin@emagaza.local ile KAYIT OL → otomatik Admin olur) |
| MongoDB verileri | http://localhost:8081 (mongo-express) |
| RabbitMQ paneli | http://localhost:15673 (guest / guest) |

## Dersler (bu sırayla oku)
| # | Dosya | Konu | Durum |
|---|-------|------|-------|
| 01 | [01-solution-yapisi.md](01-solution-yapisi.md) | .NET Solution, katmanlar, referanslar, DI | ✅ |
| 02 | [02-ef-core.md](02-ef-core.md) | EF Core, DbContext, migration, SQLite | ✅ |
| 03 | [03-unit-test.md](03-unit-test.md) | xUnit, unit vs entegrasyon testi | ✅ |
| 04 | [04-web-mvc-tasarim.md](04-web-mvc-tasarim.md) | MVC, Session sepet, CSS tasarımı | ✅ |
| 05 | [05-docker.md](05-docker.md) | Dockerfile, compose, volume | ✅ |
| 06 | [06-git-github-cicd.md](06-git-github-cicd.md) | Git akışı + GitHub Actions CI | ✅ |
| 07 | [07-mongodb.md](07-mongodb.md) | MongoDB: ürün yorumları | ✅ |
| 08 | [08-rabbitmq.md](08-rabbitmq.md) | RabbitMQ: sipariş olayı → Worker → e-posta | ✅ |
| 09 | LiteLLM + AI Memory (alışveriş asistanı) | — | ⏳ |
| 10 | LangFlow + Claude CLI/Skills/MCP | — | ⏳ |

## Mimari özet
```
Web (MVC) ──► Application (iş akışı, arayüzler) ──► Domain (kurallar)
   │                      ▲
   └──► Infrastructure ───┘  (EF Core+SQLite, MongoDB, RabbitMQ: arayüzleri uygular)
Worker ──► Application/Infrastructure   (kuyruğu dinleyen ayrı süreç)

Sipariş akışı:  Web ─SQLite'a yaz─► RabbitMQ [order-placed] ─► Worker ─► e-posta (outbox/)
Yorum akışı:    Web ◄──► MongoDB [reviews]
```
