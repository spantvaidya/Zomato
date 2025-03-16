using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Zomato.Services.RewardAPI.Message;
using Zomato.Services.RewardAPI.Services;

namespace Zomato.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreated_Reward_Subscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        ServiceBusProcessor _rewardsprocessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:orderCreatedTopic");
            orderCreated_Reward_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Reward_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardsprocessor = client.CreateProcessor(orderCreatedTopic, orderCreated_Reward_Subscription);
            _rewardService = rewardService;
        }

        public async Task Start()
        {
            //cart email processor
            _rewardsprocessor.ProcessMessageAsync += OnOrderCreatedRewardsRequestReceived;
            _rewardsprocessor.ProcessErrorAsync += ErrorHandler;
            await _rewardsprocessor.StartProcessingAsync();            
        }
        public async Task Stop()
        {
            await _rewardsprocessor.StopProcessingAsync();
            await _rewardsprocessor.DisposeAsync();
        }
        private async Task OnOrderCreatedRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive a message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                await _rewardService.UpdateRewards(objMessage);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }

    }
}
