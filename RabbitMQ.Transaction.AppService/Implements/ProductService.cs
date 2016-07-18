using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using RabbitMQ.Transaction.AppService.Entities;
using RabbitMQ.Transaction.AppService.Interfaces;

namespace RabbitMQ.Transaction.AppService.Implements
{
    /// <summary>
    /// 商品管理服务实现
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly DbSession _dbSession;

        public ProductService(DbSession dbSession)
        {
            this._dbSession = dbSession;
        }


        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <param name="price">商品价格</param>
        /// <returns>商品Id</returns>
        [OperationBehavior(TransactionScopeRequired = true)]
        public int CreateProduct(string productName, decimal price)
        {
            Product product = new Product(productName, price);
            this._dbSession.Set<Product>().Add(product);


            throw new InvalidCastException("OK");
            this._dbSession.SaveChanges();

            return product.Id;
        }

        /// <summary>
        /// 获取商品集
        /// </summary>
        /// <returns>商品集</returns>
        public IEnumerable<Product> GetProducts()
        {
            return this._dbSession.Set<Product>().AsEnumerable();
        }
    }
}
