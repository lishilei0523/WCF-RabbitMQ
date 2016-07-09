using System;
using System.ServiceModel;
using RabbitMQ.WCF.Server.Interfaces;

namespace RabbitMQ.WCF.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IProductService> factory = new ChannelFactory<IProductService>(typeof(IProductService).FullName);

            IProductService productService = factory.CreateChannel();

            string product = productService.GetProducts();

            Console.WriteLine(product);


            Guid newProductId = productService.CreateProduct("新商品");

            Console.WriteLine(newProductId);

            Console.ReadKey();
        }
    }
}
