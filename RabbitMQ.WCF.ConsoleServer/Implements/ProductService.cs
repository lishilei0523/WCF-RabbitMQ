using System;
using RabbitMQ.WCF.ConsoleServer.Interfaces;

namespace RabbitMQ.WCF.ConsoleServer.Implements
{
    /// <summary>
    /// 商品管理服务实现
    /// </summary>
    public class ProductService : IProductService
    {
        /// <summary>
        /// 获取商品集
        /// </summary>
        /// <returns>商品集</returns>
        public string GetProducts()
        {
            return "Hello World";
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <returns>商品Id</returns>
        public Guid CreateProduct(string productName)
        {
            return Guid.NewGuid();
        }
    }
}
