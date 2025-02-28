Para executar é necessario subir o RabbitMQ atraves do Doker

docker run -d --hostname rabbitmq-server --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
