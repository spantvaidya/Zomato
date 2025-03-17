using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Zomato.Web.Models;
using Zomato.Web.Services.IService;
using Zomato.Web.Utility;

namespace Zomato.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ZomatoAPI");
                HttpRequestMessage message = new();
                //responseMessage.Headers.Add("Accept", "application/json");
                if (requestDto.ContentType == SD.ContentType.MultipartFormData)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                }

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);
                message.Method = new HttpMethod(requestDto.Apitype.ToString());

                //Checming contenttype for Product Image
                if (requestDto.ContentType == SD.ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();
                    foreach (var prop in requestDto.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDto.Data);
                        if (value is FormFile)
                        {
                            var file = (FormFile)value;
                            if (file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                            }
                        }
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        }
                    }
                    message.Content = content;
                }
                else
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await client.SendAsync(message);
                if (response == null)
                {
                    return new ResponseDto { StatusCode = 500, Message = "Internal Server Error", IsSuccess = false };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    var apiContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return new ResponseDto { StatusCode = 500, Message = apiResponse.Message, IsSuccess = false };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var apiContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
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
            catch (Exception ex)
            {
                return new ResponseDto { StatusCode = 500, Message = "Internal Server Error", IsSuccess = false };
            }
            return new ResponseDto();
        }
    }
}
