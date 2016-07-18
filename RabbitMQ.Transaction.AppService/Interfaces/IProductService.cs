using System.Collections.Generic;
using System.ServiceModel;
using RabbitMQ.Transaction.AppService.Entities;

namespace RabbitMQ.Transaction.AppService.Interfaces
{
    /// <summary>
    /// 商品管理服务接口
    /// </summary>
    [ServiceContract]
    public interface IProductService
    {
        /// <summary>
        /// 创建商品
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <param name="price">商品价格</param>
        /// <returns>商品Id</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        int CreateProduct(string productName, decimal price);

        /// <summary>
        /// 获取商品集
        /// </summary>
        /// <returns>商品集</returns>
        [OperationContract]
        IEnumerable<Product> GetProducts();
    }
}
