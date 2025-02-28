using Zomato.Web.Models;

namespace Zomato.Web.Services.IService
{
    public interface IBaseService
    {
        Task <ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer);
    }
}
