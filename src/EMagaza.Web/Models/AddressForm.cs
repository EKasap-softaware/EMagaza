using System.ComponentModel.DataAnnotations;

namespace EMagaza.Web.Models;

public class AddressForm
{
    [Required(ErrorMessage = "Başlık zorunlu (ör. Ev, İş)")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad soyad zorunlu")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunlu")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "İl zorunlu")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres zorunlu")]
    public string Line { get; set; } = string.Empty;
}
