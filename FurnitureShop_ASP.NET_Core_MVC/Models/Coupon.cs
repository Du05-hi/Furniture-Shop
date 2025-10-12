using System;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "⚠️ Mã giảm giá không được để trống!")]
        [StringLength(50, ErrorMessage = "Mã giảm giá tối đa 50 ký tự.")]
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Phần trăm giảm phải từ 1 đến 100%.")]
        [Display(Name = "Phần trăm giảm giá (%)")]
        public int DiscountPercent { get; set; }

        [Display(Name = "Ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Trạng thái kích hoạt")]
        public bool IsActive { get; set; } = true;

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
