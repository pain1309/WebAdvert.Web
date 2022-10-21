using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.Repositories;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IAdvertApiClient _advertApiClient;
        private readonly IFileUploader _fileUploader;
        public AdvertManagementController(IAdvertApiClient advertApiClient, IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient;
        }
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if(ModelState.IsValid)
            {
                var id = "11111";

                // You must make a call to Advert Api, create the advertisement in the database and return Id

                var fileName = "";
                if (imageFile != null)
                {
                    fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;

                }
            }
        }
    }
}
