namespace Zomato.Services.EmailAPI.Message
{
    public class EmailMessage
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int Points { get; set; }
        public int OrderId { get; set; }
    }
}
