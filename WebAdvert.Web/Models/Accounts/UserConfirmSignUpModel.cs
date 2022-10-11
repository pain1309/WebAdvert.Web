namespace WebAdvert.Web.Models.Accounts
{
    public class UserConfirmSignUpModel
    {
        public string ConfirmationCode { get; set; }
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
    }
}
