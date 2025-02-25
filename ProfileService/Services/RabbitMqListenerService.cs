using RabbitMQ.Client;

namespace ProfileService.Services
{
    public class RabbitMqListenerService : BackgroundService
    {


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Parámetros de conexión
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string queueName = "mi_cola";

                // Declarar la cola
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Mensaje recibido: {message}");
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                // Esperar mientras se reciben mensajes
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Puedes realizar otras tareas aquí si es necesario.
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
