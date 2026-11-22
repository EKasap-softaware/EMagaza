using System.ComponentModel.DataAnnotations;

namespace EMagaza.Web.Models;

public class LoginForm
{
    [Required(ErrorMessage = "E-posta zorunlu")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunlu")]
    public string Password { get; set; } = string.Empty;
}
