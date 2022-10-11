namespace WebAdvert.Web.Models.Accounts
{
    public class UserSignUpResponse : BaseResponseModel
    {
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
    }
}
