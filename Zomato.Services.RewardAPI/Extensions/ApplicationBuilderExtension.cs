
namespace Zomato.Services.RewardAPI.Extensions
{
    public static class ApplicationBuilderExtension
    {
        private static IServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IServiceBusConsumer>();
            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(OnStarted);
            lifetime.ApplicationStopping.Register(OnStopping);
            return app;
        }

        private static void OnStarted()
        {
            ServiceBusConsumer.Start();
        }
        private static void OnStopping()
        {
            ServiceBusConsumer.Stop();
        }

    }
}
