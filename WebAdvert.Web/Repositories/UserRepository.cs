using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
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
        private readonly CognitoUserPool _userPool;
        private readonly UserContextManager _userManager;
        private readonly HttpContext _httpContext;

        public UserRepository(IOptions<AppConfig> appConfig, UserContextManager userManager, IHttpContextAccessor httpContextAccessor)
        {
            _cloudConfig = appConfig.Value;
            _provider = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(_cloudConfig.Region));
            _userPool = new CognitoUserPool(_cloudConfig.UserPoolId, _cloudConfig.AppClientId, _provider);
            _userManager = userManager;
            _httpContext = httpContextAccessor.HttpContext;
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

        public async Task<AuthResponseModel> TryLoginAsync(LoginModel model)
        {
            try
            {
                var result = await AuthenticateUserAsync(model.Email, model.Password);

                if (result.Item1.Username != null)
                {
                    await _userManager.SignIn(_httpContext, new Dictionary<string, string>() {
                        {ClaimTypes.Email, result.Item1.UserID},
                        {ClaimTypes.NameIdentifier, result.Item1.Username}
                    });
                }

                var authResponseModel = new AuthResponseModel();
                authResponseModel.EmailAddress = result.Item1.UserID;
                authResponseModel.UserId = result.Item1.Username;
                authResponseModel.Tokens = new TokenModel
                {
                    IdToken = result.Item2.IdToken,
                    AccessToken = result.Item2.AccessToken,
                    ExpiresIn = result.Item2.ExpiresIn,
                    RefreshToken = result.Item2.RefreshToken
                };
                authResponseModel.IsSuccess = true;
                return authResponseModel;
            }
            catch (UserNotConfirmedException)
            {
                var listUsersResponse = await FindUsersByEmailAddress(model.Email);

                if (listUsersResponse != null && listUsersResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    var users = listUsersResponse.Users;
                    var filtered_user = users.FirstOrDefault(x => x.Attributes.Any(x => x.Name == "email" && x.Value == model.Email));

                    var resendCodeResponse = await _provider.ResendConfirmationCodeAsync(new ResendConfirmationCodeRequest
                    {
                        ClientId = _cloudConfig.AppClientId,
                        Username = filtered_user.Username
                    });

                    if (resendCodeResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        return new AuthResponseModel
                        {
                            IsSuccess = false,
                            Message = $"Confirmation Code sent to {resendCodeResponse.CodeDeliveryDetails.Destination} via {resendCodeResponse.CodeDeliveryDetails.DeliveryMedium.Value}",
                            Status = CognitoStatusCodes.USER_UNCONFIRMED,
                            UserId = filtered_user.Username
                        };
                    }
                    else
                    {
                        return new AuthResponseModel
                        {
                            IsSuccess = false,
                            Message = $"Resend Confirmation Code Response: {resendCodeResponse.HttpStatusCode.ToString()}",
                            Status = CognitoStatusCodes.API_ERROR,
                            UserId = filtered_user.Username
                        };
                    }
                }
                else
                {
                    return new AuthResponseModel
                    {
                        IsSuccess = false,
                        Message = "No Users found for the EmailAddress.",
                        Status = CognitoStatusCodes.USER_NOTFOUND
                    };
                }
            }
            catch (UserNotFoundException)
            {
                return new AuthResponseModel
                {
                    IsSuccess = false,
                    Message = "EmailAddress not found.",
                    Status = CognitoStatusCodes.USER_NOTFOUND
                };
            }
            catch (NotAuthorizedException)
            {
                return new AuthResponseModel
                {
                    IsSuccess = false,
                    Message = "Incorrect username or password",
                    Status = CognitoStatusCodes.API_ERROR
                };
            }
        }
        private async Task<Tuple<CognitoUser, AuthenticationResultType>> AuthenticateUserAsync(string emailAddress, string password)
        {
            try
            {
                CognitoUser user = new CognitoUser(emailAddress, _cloudConfig.AppClientId, _userPool, _provider);
                InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
                {
                    Password = password
                };

                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest);
                var result = authResponse.AuthenticationResult;
                // return new Tuple<string, string, AuthenticationResultType>(user.UserID, user.Username, result);
                return new Tuple<CognitoUser, AuthenticationResultType>(user, result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<ListUsersResponse> FindUsersByEmailAddress(string emailAddress)
        {
            ListUsersRequest listUsersRequest = new ListUsersRequest
            {
                UserPoolId = _cloudConfig.UserPoolId,
                Filter = $"email=\"{emailAddress}\""
            };
            return await _provider.ListUsersAsync(listUsersRequest);
        }
    }
}
