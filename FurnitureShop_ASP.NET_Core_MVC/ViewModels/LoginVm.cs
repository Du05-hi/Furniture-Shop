using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.ViewModels;

public class LoginVm
{
    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
