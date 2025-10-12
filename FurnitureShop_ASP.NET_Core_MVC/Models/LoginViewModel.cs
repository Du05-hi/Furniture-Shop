using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;
    }
}
