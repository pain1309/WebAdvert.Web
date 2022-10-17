using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using WebAdvert.Web.Repositories;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IAdvertApiClient _advertApiClient;
        public AdvertManagementController(IAdvertApiClient advertApiClient)
        {
            _advertApiClient = advertApiClient;
        }
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }
    }
}
