<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var factory = new ConnectionFactory
{
    HostName = "localhost",
	Port = 5672,
    DispatchConsumersAsync = true
};

factory.UserName = "guest";
factory.Password = "guest";

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var message = JsonConvert.SerializeObject(new { IsIncremental = false });

channel.QueueDeclare("sync", false, false, false, null); // Ensure Queue Exists
channel.BasicPublish("", "sync", null, Encoding.UTF8.GetBytes(message));