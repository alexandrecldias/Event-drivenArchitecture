using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/pedido", async (HttpContext context) =>
{
    var pedido = new { Id = Guid.NewGuid(), Cliente = "Caratuzza", Valor = 150.75 };
    var message = JsonSerializer.Serialize(pedido);

    var factory = new ConnectionFactory() { HostName = "localhost" };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    channel.QueueDeclare(queue: "pedidos",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "",
                         routingKey: "pedidos",
                         basicProperties: null,
                         body: body);

    return Results.Ok($"Pedido {pedido.Id} enviado para processamento!");
});

app.Run();
