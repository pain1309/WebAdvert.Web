using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Repositories
{
    public interface IUserRepository
    {
        Task<UserSignUpResponse> CreateUserAsync(SignupModel signupModel);
        Task<UserSignUpResponse> ConfirmUserSignUpAsync(UserConfirmSignUpModel model);
    }
}
