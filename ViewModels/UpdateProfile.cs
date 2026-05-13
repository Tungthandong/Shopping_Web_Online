using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.ViewModels
{
    public class UpdateProfile
    {
        [StringLength(50, ErrorMessage = "Họ tên tối đa 50 ký tự")]
        public string? Fullname { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(50, ErrorMessage = "Email tối đa 50 ký tự")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải có 10-11 chữ số")]
        public string? Phonenumber { get; set; }

        public string? Birthdate { get; set; }

        public string? Gender { get; set; }
    }
}
