namespace Zomato.Services.EmailAPI
{
    public interface IServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
