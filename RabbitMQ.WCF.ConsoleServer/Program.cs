using System;
using System.ServiceModel;
using RabbitMQ.WCF.ConsoleServer.Implements;

namespace RabbitMQ.WCF.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ProductService));
            host.Open();

            Console.WriteLine("服务已启动...");
            Console.ReadKey();
        }
    }
}
