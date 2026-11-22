using System.ComponentModel.DataAnnotations;

namespace EMagaza.Web.Models;

public class CheckoutForm
{
    [Required(ErrorMessage = "Ad soyad zorunlu")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunlu"), EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunlu")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "İl zorunlu")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres zorunlu")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kart numarası zorunlu")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kart üzerindeki isim zorunlu")]
    public string CardHolder { get; set; } = string.Empty;

    [Required(ErrorMessage = "Son kullanma tarihi zorunlu")]
    public string Expiry { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVC zorunlu")]
    public string Cvc { get; set; } = string.Empty;
}
