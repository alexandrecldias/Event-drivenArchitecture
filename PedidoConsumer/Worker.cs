using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "pedidos",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var pedido = JsonSerializer.Deserialize<Pedido>(message);

            Console.WriteLine($"📦 Pedido Recebido: {pedido.Id}, Cliente: {pedido.Cliente}, Valor: {pedido.Valor:C}");
        };

        channel.BasicConsume(queue: "pedidos",
                             autoAck: true,
                             consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

public class Pedido
{
    public Guid Id { get; set; }
    public string Cliente { get; set; }
    public double Valor { get; set; }
}
