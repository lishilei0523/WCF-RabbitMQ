using RabbitMQ.WCF.IAppService.Interfaces;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RabbitMQ.WCF.Client
{
    class Program
    {
        static async Task Main()
        {
            Type orderServiceType = typeof(IOrderService);
            string endpointConfigurationName = orderServiceType.FullName;
            ChannelFactory<IOrderService> orderServiceFactory = new ChannelFactory<IOrderService>(endpointConfigurationName);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int index = 1; index <= 1000; index++)
            {
                IOrderService orderService = orderServiceFactory.CreateChannel();

                string orderNo = $"编号{index:D4}";
                string result = await Task.Run(() => orderService.CreateOrder(orderNo));
                Console.WriteLine(result);
            }

            stopwatch.Stop();

            Console.WriteLine("==============================================");
            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadKey();
        }
    }
}
