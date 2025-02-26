using Newtonsoft.Json;
using ProfileService.Controllers;
using ProfileService.DTO;
using ProfileService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ProfileService.Services
{
    public class RabbitMqListenerService : BackgroundService
    {
        private readonly HistoryService _historyService;

        public RabbitMqListenerService(HistoryService historyService) {
            this._historyService = historyService;
        }    

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

                ConnectionFactory factory = new ConnectionFactory();
                // "guest"/"guest" by default, limited to localhost connections
                factory.UserName = "guest";
                factory.Password = "guest";
                factory.VirtualHost = "/";
                factory.HostName = "localhost";

                IConnection conn = await factory.CreateConnectionAsync();

                IChannel channel = await conn.CreateChannelAsync();

                await channel.ExchangeDeclareAsync("profile_exchange", ExchangeType.Direct);
                await channel.QueueDeclareAsync("profile_queue", false, false, false, null);
                await channel.QueueBindAsync("profile_queue", "profile_exchange", "update_history", null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    // copy or deserialise the payload
                    // and process the message
                    // ...
                    string message = Encoding.UTF8.GetString(body);
                    Debug.WriteLine(message);
                    Console.WriteLine(message);
                    Debug.WriteLine(ea.RoutingKey);
                    Console.WriteLine(ea.RoutingKey);
                    //await channel.BasicAckAsync(ea.DeliveryTag, false);
                    try
                    {
                        switch (ea.RoutingKey)
                        {
                            case "profile_queue":
                                DtoUpdHistory dtoHistory = JsonConvert.DeserializeObject<DtoUpdHistory>(message);
                                bool bien = await _historyService.updateHistory(new DTO.DtoUpdHistory());
                                if (bien)
                                {
                                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                                    Console.WriteLine("Se actualizo el historial de articulos");
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error procesando mensaje: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, false); 
                    }
                };
                // this consumer tag identifies the subscription
                // when it has to be cancelled
                string consumerTag = await channel.BasicConsumeAsync("profile_queue", false, consumer);
                await Task.Delay(Timeout.Infinite, stoppingToken);

        }
    }
}