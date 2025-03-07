using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Zomato.Services.EmailAPI.Models.Dto;
using Zomato.Services.EmailAPI.Services;

namespace Zomato.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailRegisterUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        ServiceBusProcessor _emailCartprocessor;
        ServiceBusProcessor _emailRegisterprocessor;


        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailCartQueue");
            emailRegisterUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisterQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartprocessor = client.CreateProcessor(emailCartQueue);
            _emailRegisterprocessor = client.CreateProcessor(emailRegisterUserQueue);
            _emailService = emailService;
        }

        public async Task Start()
        {
            //cart email processor
            _emailCartprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartprocessor.StartProcessingAsync();
            
            //register email processor
            _emailRegisterprocessor.ProcessMessageAsync += OnEmailRegisterUserReceived;
            _emailRegisterprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailRegisterprocessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _emailCartprocessor.StopProcessingAsync();
            await _emailCartprocessor.DisposeAsync();

            await _emailRegisterprocessor.StopProcessingAsync();
            await _emailRegisterprocessor.DisposeAsync();
        }
        private async Task OnEmailRegisterUserReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive a message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var objMessage = JsonConvert.DeserializeObject<RegisterationDto>(body);

            try
            {
                await _emailService.SendAndLogRegisterUserEmailAsync(objMessage);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive a message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {                
                await _emailService.SendAndLogEmailCartAsync(objMessage);

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
