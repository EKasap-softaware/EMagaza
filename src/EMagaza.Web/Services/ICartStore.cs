using EMagaza.Application.Carts;

namespace EMagaza.Web.Services;

/// <summary>Sepetin nerede saklandığını (oturum, cookie, veritabanı...) controller'lardan gizler.</summary>
public interface ICartStore
{
    Cart Get();
    void Save(Cart cart);
}
