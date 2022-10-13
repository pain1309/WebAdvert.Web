using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "ConfirmationCode is required.")]
        [Display(Name = "ConfirmationCode")]
        public string ConfirmationCode { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
