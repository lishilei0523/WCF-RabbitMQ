using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.8.210";
            factory.UserName = "admin";
            factory.Password = "123456";

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, e) =>
                    {
                        byte[] body = e.Body;
                        string message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Received {0}", message);
                    };

                    channel.BasicConsume("hello", true, consumer);
                    Console.WriteLine(" Press any key to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
