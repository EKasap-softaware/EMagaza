using System.Text.Json;
using EMagaza.Application.Carts;

namespace EMagaza.Web.Services;

public class SessionCartStore(IHttpContextAccessor accessor) : ICartStore
{
    private const string SessionKey = "cart";

    private ISession Session => accessor.HttpContext!.Session;

    public Cart Get() =>
        Session.GetString(SessionKey) is { } json ? JsonSerializer.Deserialize<Cart>(json) ?? new Cart() : new Cart();

    public void Save(Cart cart) => Session.SetString(SessionKey, JsonSerializer.Serialize(cart));
}
