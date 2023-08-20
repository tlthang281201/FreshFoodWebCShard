using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ModelView
{
    public class PlaceOrderVM
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Account")]
        public string Email { get; set; }
        [MaxLength(11)]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Account")]
        public string Phone { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Phường")]
        [Required(ErrorMessage = "Vui lòng chọn phường")]
        public string Ward { get; set; }
        [Display(Name = "Quận")]
        [Required(ErrorMessage = "Vui lòng chọn quận")]
        public string District { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
        [Required(ErrorMessage = "Vui lòng phương thức thanh toán")]
        public int PaymentId { get; set; }
    }
}
