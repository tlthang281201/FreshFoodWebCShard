using System.ComponentModel.DataAnnotations;

namespace ShoeShop.ModelView
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name ="Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        public string PasswordNow { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(5,ErrorMessage = "Mật khẩu tối thiểu 5 kí tự")]
        public string Password { get; set; }
        [Display(Name = "Nhập lại mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 5 kí tự")]
        [Compare("Password",ErrorMessage ="Mật khẩu không giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}
