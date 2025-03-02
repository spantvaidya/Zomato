namespace Zomato.Services.CartAPI.Models.Dto
{
    public class ResponseDto
    {
        public object Result { get; set; }
        public string Message { get; set; } = "Success";
        public bool IsSuccess { get; set; } = true;
        public int StatusCode { get; set; }
    }
}
