using Newtonsoft.Json;
using System.Text;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ZomatoAPI");
                HttpResponseMessage message = new HttpResponseMessage();
                //message.Headers.Add("Content-Type", "application/json");
                message.Headers.Add("Accept", "application/json");

                message.RequestMessage = new HttpRequestMessage
                {
                    Method = new HttpMethod(requestDto.Apitype.ToString()),
                    RequestUri = new Uri(requestDto.Url),
                    Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await client.SendAsync(message.RequestMessage);
                var apiContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                return apiResponse;
            }
            catch(Exception ex)
            {
                return new ResponseDto { StatusCode = 500, Message = ex.Message + " " + ex.InnerException, IsSuccess = false };
            }
        }
    }
}
