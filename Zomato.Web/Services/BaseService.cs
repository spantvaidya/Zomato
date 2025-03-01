using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;

namespace Zomato.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory,ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ZomatoAPI");
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                //message.Headers.Add("Content-Type", "application/json");
                responseMessage.Headers.Add("Accept", "application/json");

                responseMessage.RequestMessage = new HttpRequestMessage
                {
                    Method = new HttpMethod(requestDto.Apitype.ToString()),
                    RequestUri = new Uri(requestDto.Url),
                    Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json")
                };

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    //HttpRequestMessage requestMessage = new HttpRequestMessage();
                    //requestMessage.Headers.Add("Authorization",$"Bearer {token}");
                    responseMessage.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.SendAsync(responseMessage.RequestMessage);
                if(response == null)
                {             
                    return new ResponseDto { StatusCode = 500, Message = "Internal Server Error", IsSuccess = false };
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    var apiContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return new ResponseDto { StatusCode = 500, Message = apiResponse.Message, IsSuccess = false };
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ResponseDto { StatusCode = 404, Message = "Not Found", IsSuccess = false };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ResponseDto { StatusCode = 401, Message = "Unauthorized", IsSuccess = false };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ResponseDto { StatusCode = 403, Message = "Forbidden", IsSuccess = false };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var apiContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return new ResponseDto { StatusCode = 400, Message = apiResponse.Message, IsSuccess = false };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return apiResponse;
                }
            }
            catch(Exception ex)
            {
                return new ResponseDto { StatusCode = 500, Message = "Internal Server Error", IsSuccess = false };
            }
            return new ResponseDto();
        }
    }
}
