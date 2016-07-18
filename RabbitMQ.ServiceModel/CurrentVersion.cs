using System;
using System.Text;

using RabbitMQ.Client.Framing;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// Properties of the current RabbitMQ Service Model Version
    /// </summary>
    public static class CurrentVersion
    {
        internal const String Scheme = "soap.amqp";

        internal static Encoding DefaultEncoding { get { return Encoding.UTF8; } }

        internal static class StatusCodes
        {
            public const int Ok = Constants.ReplySuccess;
        }

    }
}
