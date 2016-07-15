using System;
using System.ServiceModel;
using RabbitMQ.WCF.Server.Interfaces;

namespace RabbitMQ.WCF.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("商品部分");

            //ChannelFactory<IProductService> productSvcFactory = new ChannelFactory<IProductService>(typeof(IProductService).FullName);
            //IProductService productService = productSvcFactory.CreateChannel();

            //string product = productService.GetProducts();
            //Console.WriteLine(product);
            //Guid newProductId = productService.CreateProduct("新商品");
            //Console.WriteLine(newProductId);
            //Console.WriteLine("=========================================================");
            //Console.WriteLine();

            Console.WriteLine("订单部分");

            ChannelFactory<IOrderService> orderSvcFactory = new ChannelFactory<IOrderService>(typeof(IOrderService).FullName);
            IOrderService orderService = orderSvcFactory.CreateChannel();
            orderService.CreateOrder(Guid.NewGuid().ToString());
            Console.WriteLine("=========================================================");
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
