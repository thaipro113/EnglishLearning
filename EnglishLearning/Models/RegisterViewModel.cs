using System.ComponentModel.DataAnnotations;

namespace EnglishLearning.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ và số, không có khoảng trắng hoặc ký tự đặc biệt")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải từ 4-100 ký tự")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [RegularExpression(@"^(?!\s)([A-Za-zÀ-ỹ\s]+)(?<!\s)$",
    ErrorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng, không được có khoảng trắng ở đầu/cuối")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại chỉ được chứa 10 chữ số")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
