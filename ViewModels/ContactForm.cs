using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.ViewModels
{
    public class ContactForm
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public IFormFile File { get; set; }  
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
