using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Repositories
{
    public interface IUserRepository
    {
        Task<UserSignUpResponse> CreateUserAsync(SignupModel signupModel);
        Task<UserSignUpResponse> ConfirmUserSignUpAsync(UserConfirmSignUpModel model);
        Task<AuthResponseModel> TryLoginAsync(LoginModel model);
        Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ForgotPasswordModel model);
        Task<ForgotPasswordResponseModel> ConfirmForgotPasswordAsync(ConfirmForgotPasswordModel model);
    }
}
