# 05 · Docker

Doğrulandı: `docker compose up -d --build` → http://localhost:8080 çalışıyor, 12 ürün listeleniyor, imaj ≈ 385 MB.

## Temel kavramlar
- **Image** = tarif + dondurulmuş dosya sistemi (değişmez). **Container** = imajın çalışan örneği.
- **Dockerfile** = imajın nasıl yapılacağı. **Compose** = birden çok container + ağ + volume'u tek YAML ile tanımlama.
- **Volume** = container silinse de duran veri.

## Dockerfile satır satır (multi-stage)
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build     # derleme imajı: büyük (SDK içerir)
COPY src/EMagaza.Domain/*.csproj src/EMagaza.Domain/   # ... sadece csproj'lar
RUN dotnet restore                                  # paketler bu katmanda cache'lenir
COPY . .                                            # kod şimdi gelir
RUN dotnet publish src/EMagaza.Web -c Release -o /app --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:9.0            # çalışma imajı: küçük (sadece runtime)
COPY --from=build /app .                            # derleme aşamasından sadece çıktıyı al
ENV ASPNETCORE_URLS=http://+:8080                   # container'da 80 değil 8080 (root olmayan kullanıcı uyumlu)
ENTRYPOINT ["dotnet", "EMagaza.Web.dll"]
```
**Neden csproj'ları önce kopyalıyoruz?** Docker her satırı bir *katman* olarak cache'ler. Kodu değiştirince `COPY . .` geçersiz olur ama `restore` katmanı değişmez → yeniden paket indirmez. Sıra = hız.
**Neden multi-stage?** SDK (~800 MB) üretime gitmez; son imajda sadece runtime kalır.
`.dockerignore` (`bin/ obj/ .git`) → build context küçülür, yerel `obj` imaja sızmaz.

## docker-compose.yml
```yaml
ports: ["8080:8080"]        # sunucu:container
environment:
  ConnectionStrings__Store: "Data Source=/data/store.db"   # __ = config hiyerarşisinde ':' (ConnectionStrings:Store)
volumes: [store-data:/data] # SQLite dosyası volume'da → sipariş kalıcı
```
Ortam değişkeni `appsettings.json`'ı ezer: aynı imaj dev/test/prod'da farklı ayarla çalışır (12-factor).

## Komutlar
```powershell
docker compose up -d --build     # derle + arka planda başlat
docker compose logs -f web       # canlı log
docker compose ps                # durum
docker compose down              # durdur (volume kalır)
docker compose down -v           # + volume'u da sil (veri gider!)
docker exec -it emagaza-web-1 sh # içine gir
```
## Sık hatalar
- Uygulama `localhost`'u kendi container'ı sanır: başka container'a (Mongo, RabbitMQ) servis **adıyla** bağlan (`mongo:27017`), `localhost` değil.
- Volume'suz SQLite: container silinince veri gider.
- `localhost:5020`'de çalışan kendi sürümünle 8080 karıştırmak (iki ayrı veritabanı dosyası!).
## Mülakat soruları
Image/container farkı? Layer cache nasıl çalışır? Multi-stage neden? Volume vs bind mount? EXPOSE gerçekten port açar mı (hayır, sadece belgeler; `ports` açar)? Compose'ta servisler birbirini nasıl bulur (DNS ile servis adı)?
