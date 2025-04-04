using System.ComponentModel.DataAnnotations;

namespace KeyManagement.ViewModels;

public class CreateKeyViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Введите логин")]
    public string? Login { get; set; }
    
    [Required(ErrorMessage = "Введите срок действия")]
    [Range(1, int.MaxValue, ErrorMessage = "Срок действия должен быть не менее 1 дня")]
    public int Expires { get; set; }

    public Guid Key { get; set; } = Guid.NewGuid();
}