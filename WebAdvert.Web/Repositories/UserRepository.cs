using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Options;
using WebAdvert.Web.Contracts;
using WebAdvert.Web.Enum;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppConfig _cloudConfig;
        private readonly AmazonCognitoIdentityProviderClient _provider;
        public UserRepository(IOptions<AppConfig> appConfig)
        {
            _cloudConfig = appConfig.Value;
            _provider = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(_cloudConfig.Region));
        }

        public async Task<UserSignUpResponse> CreateUserAsync(SignupModel signupModel)
        {
            var signUpRequest = new SignUpRequest()
            {
                ClientId = _cloudConfig.AppClientId,
                Password = signupModel.Password,
                Username = signupModel.Email
            };

            signUpRequest.UserAttributes.Add(new AttributeType
            {
                Name = "name",
                Value = signupModel.Email
            });

            SignUpResponse response = await _provider.SignUpAsync(signUpRequest);

            var signUpResponse = new UserSignUpResponse()
            {
                UserId = response.UserSub,
                EmailAddress = signupModel.Email,
                Message = $"Confirmation Code sent to {response.CodeDeliveryDetails.Destination} via {response.CodeDeliveryDetails.DeliveryMedium.Value}",
                Status = CognitoStatusCodes.USER_UNCONFIRMED,
                IsSuccess = true
            };

            return signUpResponse;
        }

        public async Task<UserSignUpResponse> ConfirmUserSignUpAsync(UserConfirmSignUpModel model)
        {
            ConfirmSignUpRequest request = new ConfirmSignUpRequest()
            {
                ClientId = _cloudConfig.AppClientId,
                ConfirmationCode = model.ConfirmationCode,
                Username = model.EmailAddress
            };

            var response = await _provider.ConfirmSignUpAsync(request);

            return new UserSignUpResponse
            {
                EmailAddress = model.EmailAddress,
                UserId = model.UserId,
                Message = "User Confirmed",
                IsSuccess = true
            };
        }

    }
}
