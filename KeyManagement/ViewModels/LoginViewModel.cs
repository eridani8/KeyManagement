using System.ComponentModel.DataAnnotations;

namespace KeyManagement.ViewModels;

public class LoginViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите логин")]
    public string? Username { get; set; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите пароль")]
    public string? Password { get; set; }
}