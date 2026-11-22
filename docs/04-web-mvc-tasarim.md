# 04 · Web: MVC, Sepet, Tasarım

## MVC akışı
`GET /Shop/Index?category=Ev` → **Routing** (`Program.cs` → `{controller=Shop}/{action=Index}/{id?}`) → `ShopController.Index` → repository'den veri → `View(model)` → `Views/Shop/Index.cshtml` (Razor) → HTML.

| Dosya | Görev |
|---|---|
| `Controllers/ShopController` | Liste, filtre, arama, detay |
| `Controllers/CartController` | Sepete ekle/çıkar (POST) |
| `Controllers/CheckoutController` | Form doğrulama + `CheckoutService` çağrısı |
| `Views/Shared/_Layout.cshtml` | Üst çubuk + sepet rozeti (tüm sayfalarda ortak iskelet) |
| `wwwroot/css/store.css` | Tüm tasarım (framework yok) |

## Sepet neden Session'da?
Giriş sistemi yok; sepet **kullanıcıya özel, geçici** veri. `CartSession.cs` `Cart` nesnesini JSON'a çevirip `ISession`'a yazar. Çerez sadece oturum kimliğini taşır, veri sunucuda.
Gerekenler: `AddSession()` + `app.UseSession()` (sıra önemli: `UseRouting`'den sonra, endpoint'ten önce).
Sınır: sunucu yeniden başlarsa sepet gider; çok sunucuda dağıtık cache (Redis) gerekir.

## Güvenlik detayları (kodda var)
- `[ValidateAntiForgeryToken]` + form `asp-` tag helper'ı otomatik token koyar → **CSRF** koruması. POST'lar GET olmasın: durum değiştiren işlem GET ile yapılmaz.
- Fiyat sepetten değil DB'den (bkz. 03).
- `DataAnnotations` (`[Required]`, `[EmailAddress]`) sunucu tarafı doğrulama; `ModelState.IsValid` kontrolü. İstemci doğrulaması **asla tek başına** yeterli değil.
- Razor çıktıyı otomatik HTML-encode eder → XSS koruması.

## Tasarım kararları
- **Dış bağımlılık yok:** ürün görseli yerine emoji + gradyan (`Product.Emoji/Color`). Offline çalışır, CDN yok.
- **CSS değişkenleri** (`:root{--brand:...}`) → tek yerden tema.
- **Grid:** `repeat(auto-fill,minmax(250px,1fr))` → medya sorgusuz duyarlı kart ızgarası.
- Hover animasyonu `transform` ile (GPU hızlı), `backdrop-filter` ile cam efektli sticky üst çubuk, `@keyframes` ile toast.
- Mobil: `@media(max-width:760px)` tek sütun.

## Mülakat soruları
MVC nedir? Routing nasıl çalışır? Middleware sırası neden önemli? CSRF/XSS nedir, ASP.NET nasıl korur? Session vs Cookie vs JWT? ViewBag/ViewData/Model farkı?
