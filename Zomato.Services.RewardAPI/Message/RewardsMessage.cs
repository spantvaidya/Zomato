﻿namespace Zomato.Services.RewardAPI.Message
{
    public class RewardsMessage
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int Points { get; set; }
        public int OrderId { get; set; }
    }
}
