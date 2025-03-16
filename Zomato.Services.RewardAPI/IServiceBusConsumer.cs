namespace Zomato.Services.RewardAPI
{
    public interface IServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
