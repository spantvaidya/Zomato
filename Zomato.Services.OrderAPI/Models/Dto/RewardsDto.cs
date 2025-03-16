namespace Zomato.Services.OrderAPI.Models.Dto
{
    public class RewardsDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int Points { get; set; }
        public int OrderId { get; set; }
    }
}
