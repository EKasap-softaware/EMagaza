# 03 · Unit Test (xUnit)

Dosya: `tests/EMagaza.Tests/StoreTests.cs` · Çalıştır: `dotnet test` → **10 test geçti**.

## İki tür test, ikisi de burada
| Tür | Sınıf | Neye dokunur | Hız |
|---|---|---|---|
| **Unit** | `CartTests`, `ProductTests` | Sadece saf C# (DB yok) | ms |
| **Entegrasyon** | `CheckoutServiceTests` | Gerçek EF Core + **bellek içi SQLite** | ~100 ms |

Neden `Cart` saf? Veritabanı/HTTP bilmiyor → test için hiçbir hazırlık gerekmez. **Test edilebilirlik tasarım kalitesinin ölçüsüdür.**

## Anatomi: Arrange – Act – Assert
```csharp
[Fact]
public void Add_SameProductTwice_MergesQuantity()
{
    var cart = new Cart();                          // Arrange
    cart.Add(1, "Kulaklık", 100m, "🎧");            // Act
    cart.Add(1, "Kulaklık", 100m, "🎧", 2);
    Assert.Equal(300m, cart.Total);                 // Assert
}
```
- `[Fact]` = tek senaryo. `[Theory]` + `[InlineData]` = aynı test farklı verilerle (`qty` = 0 ve -5).
- İsim kalıbı: `Metot_Durum_BeklenenSonuç`.

## Entegrasyon testi tuzağı: bellek içi SQLite
```csharp
_conn = new SqliteConnection("Data Source=:memory:"); _conn.Open();
```
Bağlantı **açık kalmalı**; kapanınca bellek DB'si yok olur. Bu yüzden `IDisposable` ile kapatıyoruz.
`EnsureCreated()` şemayı migration'sız kurar (test için yeterli).
Neden EF'in `InMemory` sağlayıcısı değil? O, SQL/kısıt/transaction davranışını taklit etmez, yalan söyler. SQLite gerçek bir ilişkisel motor.

## Neyi test ettik (ve neden)
1. Sipariş kaydedilir + stok düşer.
2. **Fiyat veritabanından alınır** (sepetteki 1 TL yok sayılır) → güvenlik kuralı.
3. Yetersiz stokta exception **ve hiç sipariş kaydedilmez**.
4. Boş sepet reddedilir.

## Komutlar
```powershell
dotnet test                                   # hepsi
dotnet test --filter "FullyQualifiedName~Cart"  # isme göre filtre
dotnet test --collect:"XPlat Code Coverage"     # kapsama
```
## Mock nedir? (burada kullanmadık, bil)
Bir arayüzün sahte hali (Moq/NSubstitute). Gerçek DB yerine `IProductRepository`'yi mock'larsın. Biz gerçek SQLite seçtik çünkü daha az sahte = daha güvenilir test. İkisi de meşrudur.

## Mülakat soruları
Unit vs integration? Test piramidi? AAA? Mock/Stub/Fake farkı? Neden InMemory EF sağlayıcısı önerilmez? Fact/Theory?
