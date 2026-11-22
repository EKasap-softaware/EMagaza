# 01 · .NET Solution Yapısı

## Solution nedir?
`.sln` = projeleri (`.csproj`) bir arada tutan **klasör/harita** dosyası. Kod içermez. Visual Studio ve `dotnet build` onu okuyup tüm projeleri derler.
Her `.csproj` = bir derleme birimi (DLL/EXE). Neden birden çok proje? **Bağımlılık yönünü derleyiciye zorlatmak için.** Domain projesi Web'e referans veremezse, yanlışlıkla `HttpContext` kullanamazsın.

## Bu projede
```
EMagaza.sln
├─ src/EMagaza.Domain          → Product, Order, OrderItem (+ iş kuralı: DecreaseStock). Hiçbir pakete bağımlı DEĞİL.
├─ src/EMagaza.Application     → Cart, CheckoutService, IProductRepository/IOrderRepository (arayüzler). Sadece Domain'i bilir.
├─ src/EMagaza.Infrastructure  → EF Core: StoreDbContext, repository'ler, migration, seed. Application arayüzlerini UYGULAR.
├─ src/EMagaza.Web             → MVC controller + view + CSS. Composition root (Program.cs her şeyi birbirine bağlar).
└─ tests/EMagaza.Tests         → xUnit
```
**Kural: oklar içe doğru.** `Web → Application → Domain`. `Infrastructure → Application`. Domain kimseyi bilmez.
Buna Clean Architecture / Onion denir.

## Sıfırdan komutlar (bu projeyi böyle kurdum)
```powershell
dotnet new sln -n EMagaza --format sln          # klasik .sln üretir (.NET 10 SDK'dan itibaren varsayılan .slnx olur; --format ile açıkça seçmek garanti)
dotnet new classlib -n EMagaza.Domain -o src/EMagaza.Domain -f net9.0
dotnet new classlib -n EMagaza.Application -o src/EMagaza.Application -f net9.0
dotnet new classlib -n EMagaza.Infrastructure -o src/EMagaza.Infrastructure -f net9.0
dotnet new mvc    -n EMagaza.Web   -o src/EMagaza.Web   -f net9.0
dotnet new xunit  -n EMagaza.Tests -o tests/EMagaza.Tests -f net9.0
dotnet sln add src/*/*.csproj tests/*/*.csproj  # projeleri solution'a ekle
dotnet add src/EMagaza.Application reference src/EMagaza.Domain          # proje referansı = ok yönü
dotnet add src/EMagaza.Infrastructure reference src/EMagaza.Application
dotnet add src/EMagaza.Web reference src/EMagaza.Application src/EMagaza.Infrastructure
dotnet add src/EMagaza.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.0   # NuGet paketi
```
Neden `-f net9.0`? Hedef framework'ü açıkça yazmak, ileride SDK yükseltilince şablonun sessizce başka sürüme geçmesini önler.

## Dependency Injection (DI) — en çok sorulan konu
Sınıf bağımlılığını **kendisi üretmez, dışarıdan ister**:
```csharp
public class CheckoutService(IProductRepository products, IOrderRepository orders)  // primary constructor
```
`CheckoutService` EF Core'u bilmiyor, sadece arayüzü biliyor. Gerçek sınıf `Program.cs`'te bağlanır:
`DependencyInjection.cs` → `s.AddScoped<IProductRepository, ProductRepository>();`

Ömürler (mülakat sorusu):
| Ömür | Anlamı | Örnek |
|---|---|---|
| Singleton | Uygulama boyunca 1 nesne | cache, config |
| **Scoped** | **Her HTTP isteğinde 1 nesne** | `DbContext`, repository |
| Transient | Her istenişte yeni | hafif, durumsuz servis |

`DbContext` Scoped olmalı: istek bitince atılır; Singleton yaparsan thread-safe olmadığı için patlar.

## Neden arayüz (IProductRepository)?
1. Testte sahtesini/SQLite sürümünü verebilirsin.
2. Yarın SQLite → SQL Server → MongoDB geçişinde sadece Infrastructure değişir. (Rehberin MongoDB dersinde tam bunu yapacağız.)

## Sık hatalar
- **Döngüsel referans**: Domain'e Application referansı ekleyemezsin (derleyici reddeder) — bu bir özellik, hata değil.
- `Program.cs`'te servis kaydetmeyi unutmak → çalışırken `Unable to resolve service for type ...`.
- Scoped servisi Singleton içine enjekte etmek → "captive dependency".

## Mülakat soruları
1. Solution ile project farkı? 2. Neden katmanlı? 3. DI nedir, 3 ömür? 4. Neden DbContext scoped? 5. Application neden Infrastructure'ı referanslamaz? (Dependency Inversion)
