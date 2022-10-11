using WebAdvert.Web.Enum;

namespace WebAdvert.Web.Models
{
    public class BaseResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CognitoStatusCodes Status { get; set; }
    }
}
