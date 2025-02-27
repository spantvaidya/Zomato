namespace Zomato.Web.Services.IService
{
    public interface ITokenProvider
    {
        public void ClearToken();
        public String? GetToken();
        public void SetToken(String? token);
    }
}
