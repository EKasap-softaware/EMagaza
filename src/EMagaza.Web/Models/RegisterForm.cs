using System.ComponentModel.DataAnnotations;

namespace EMagaza.Web.Models;

public class RegisterForm
{
    [Required(ErrorMessage = "Ad zorunlu")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunlu"), EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunlu"), MinLength(6, ErrorMessage = "Şifre en az 6 karakter")]
    public string Password { get; set; } = string.Empty;
}
