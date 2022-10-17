using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<AdvertResponse> Create(AdvertModel model);
        Task<bool> Confirm(ConfirmAdvertModel model);
    }
}
