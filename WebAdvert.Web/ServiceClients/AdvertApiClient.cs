using AdvertApi.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            Configuration = configuration;
            Client = client;
            Mapper = mapper;
            var createUrl = Configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");
            Client.BaseAddress = new Uri(createUrl);
            Client.DefaultRequestHeaders.Add("Content-type", "application/json");
        }

        public IConfiguration Configuration { get; }
        public HttpClient Client { get; }
        public IMapper Mapper { get; }

        public async Task<AdvertResponse> Create(AdvertModel model)
        {
            var advertApiModel = new AdvertModel();
            var jsonModel = JsonConvert.SerializeObject(advertApiModel);
            var response = await Client.PostAsync(Client.BaseAddress, new StringContent(jsonModel)).ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
            var advertResponse = new AdvertResponse();

            return advertResponse;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            var advertModel = Mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertModel);
            var response = await Client.PutAsync(new Uri($"{Client.BaseAddress}/confirm"), new StringContent(jsonModel)).ConfigureAwait(false);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
