using System.ComponentModel.DataAnnotations;

namespace EMagaza.Web.Models;

public class ChangePasswordForm
{
    [Required(ErrorMessage = "Mevcut şifre zorunlu")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifre zorunlu"), MinLength(6, ErrorMessage = "Şifre en az 6 karakter")]
    public string NewPassword { get; set; } = string.Empty;
}
