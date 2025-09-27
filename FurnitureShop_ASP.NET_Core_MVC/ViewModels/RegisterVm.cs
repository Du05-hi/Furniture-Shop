using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.ViewModels;

public class RegisterVm
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
