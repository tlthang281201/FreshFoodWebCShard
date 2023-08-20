using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ModelView
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage ="Vui lòng nhập email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5,ErrorMessage ="Mật khẩu tối thiểu 5 kí tự")]
        public string Password { get; set; }
    }
}
