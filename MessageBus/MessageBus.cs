
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Zomato.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string conenctionstring = "Endpoint=sb://zomatoweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=nfsJJOTSy5+b64pYFkgMIhq6+9OACTKmT+ASbHPfYds=";
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(conenctionstring);
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(busMessage);
            await client.DisposeAsync();
        }
    }
}
