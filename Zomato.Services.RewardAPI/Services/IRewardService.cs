using Zomato.Services.RewardAPI.Message;

namespace Zomato.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
