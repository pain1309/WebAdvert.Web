using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using WebAdvert.Web.Repositories;

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private readonly IUserRepository _userRepository;
        public Accounts(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        public IActionResult ConfirmSignup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmSignupAsync(UserConfirmSignUpModel model)
        {
            var response = await _userRepository.ConfirmUserSignUpAsync(model);

            if (response.IsSuccess)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignupAsync(SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response = await _userRepository.CreateUserAsync(model);

            if (response.IsSuccess)
            {
                TempData["UserId"] = response.UserId;
                TempData["EmailAddress"] = response.EmailAddress;
            }

            return RedirectToAction("ConfirmSignup");
        }
        
        [HttpGet]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> Login_Post(LoginModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
    }
}
