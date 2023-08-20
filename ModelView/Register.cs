using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ModelView
{
    public class Register
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; }
        [MaxLength(150)]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail",controller:"Account")]
        public string Email { get; set; }
        [MaxLength(11)]
        [Display(Name = "Điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Account")]
        public string Phone { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 5 kí tự")]
        public string Password { get; set; }
        [Display(Name = "Nhập lại mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 5 kí tự")]
        [Compare("Password",ErrorMessage ="Vui lòng nhập mật khẩu giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}
