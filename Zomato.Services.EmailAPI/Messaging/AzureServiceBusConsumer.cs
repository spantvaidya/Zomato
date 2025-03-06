using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Zomato.Services.EmailAPI.Models.Dto;

namespace Zomato.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;

        ServiceBusProcessor _emailCartprocessor;

        public AzureServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartprocessor = client.CreateProcessor(emailCartQueue);

        }

        public async Task Start()
        {
            _emailCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartprocessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _emailCartprocessor.StopProcessingAsync();
            await _emailCartprocessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive a message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //send email
                //var email = new EmailRequestDto()
                //{
                //    To = objMessage.Email,
                //    Subject = "Your Cart",
                //    Body = "Your cart is ready"
                //};
                //var emailService = new EmailService(_configuration);
                //await emailService.SendEmail(email);

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
