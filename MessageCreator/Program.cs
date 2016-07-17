using System;
using System.Text;
using RabbitMQ.Client;

namespace MessageCreator
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

                    for (int i = 0; i < 40; i++)
                    {
                        string message = "Hello World" + i;
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(string.Empty, "hello", null, body);
                        Console.WriteLine(" set {0}", message);
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
