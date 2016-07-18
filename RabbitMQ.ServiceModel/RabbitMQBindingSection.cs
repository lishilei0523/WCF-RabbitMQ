using System.ServiceModel.Configuration;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// Allows the RabbitMQBinding to be declarativley configured
    /// </summary>
    public sealed class RabbitMQBindingSection : StandardBindingCollectionElement<RabbitMQBinding, RabbitMQBindingConfigurationElement>
    {
    }
}
