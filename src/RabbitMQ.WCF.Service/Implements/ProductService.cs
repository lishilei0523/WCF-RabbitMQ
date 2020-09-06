﻿using System;
using System.ServiceModel;
using RabbitMQ.WCF.Service.Interfaces;

namespace RabbitMQ.WCF.Service.Implements
{
    /// <summary>
    /// 商品管理服务实现
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ProductService : IProductService
    {
        /// <summary>
        /// 获取商品集
        /// </summary>
        /// <returns>商品集</returns>
        public string GetProducts()
        {
            Console.WriteLine("Hello World");
            return "Hello World";
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <returns>商品Id</returns>
        public Guid CreateProduct(string productName)
        {
            Console.WriteLine(productName);
            return Guid.NewGuid();
        }
    }
}
