using System;
using System.ServiceModel;
using RabbitMQ.WCF.Server.Implements;

namespace RabbitMQ.WCF.ConsoleHost
{
    class Program
    {
        static void Main()
        {
            ServiceHost productSvcHost = new ServiceHost(typeof(ProductService));
            ServiceHost orderSvcHost = new ServiceHost(typeof(OrderService));
            
            productSvcHost.Open();
            orderSvcHost.Open();

            Console.WriteLine("服务已启动...");
            Console.ReadLine();
        }
    }
}
