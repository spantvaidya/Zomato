using Microsoft.EntityFrameworkCore;
using Zomato.Services.RewardAPI.Data;
using Zomato.Services.RewardAPI.Message;
using Zomato.Services.RewardAPI.Models;

namespace Zomato.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardService(DbContextOptions<AppDbContext> dbOptions)
        {
            this._dbOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                using (var dbContext = new AppDbContext(_dbOptions))
                {
                    var rewards = new Rewards
                    {
                        UserId = rewardsMessage.UserId,
                        Points = rewardsMessage.Points,
                        OrderId = rewardsMessage.OrderId,
                        RewardsDate = DateTime.Now
                    };
                    
                    await dbContext.Rewards.AddAsync(rewards);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {                
            }
        }
    }
}
