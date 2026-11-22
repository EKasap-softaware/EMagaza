# 🛍️ EMağaza

.NET 9 · ASP.NET Core MVC · **MongoDB** · **RabbitMQ** · Cookie kimlik doğrulama · xUnit · Docker · GitHub Actions

E-ticaret öğrenme projesi: ürün listesi/arama/kategori, sepet, kayıt/giriş, sipariş, yorum, yönetim paneli,
sipariş → RabbitMQ → Worker → e-posta kaydı.

```powershell
docker compose up -d --build   # http://localhost:8080
dotnet test                    # 139 test
```
Mongo verileri: http://localhost:8081 · RabbitMQ paneli: http://localhost:15673 (guest/guest) ·
Admin olmak için `admin@emagaza.local` e-postasıyla kayıt ol.

Öğrenme rehberi: [docs/00-INDEKS.md](docs/00-INDEKS.md)
