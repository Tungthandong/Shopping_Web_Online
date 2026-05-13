using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.ViewModels
{
    public class ContactForm
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải có 10-11 chữ số")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Range(1, 100000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [StringLength(2000, ErrorMessage = "Lời nhắn tối đa 2000 ký tự")]
        public string? Note { get; set; }

        public IFormFile? File { get; set; }

        public int ProductId { get; set; }
    }
}
