using static Zomato.Web.Utility.SD;

namespace Zomato.Web.Models
{
    public class RequestDto
    {
        public ApiType Apitype { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
