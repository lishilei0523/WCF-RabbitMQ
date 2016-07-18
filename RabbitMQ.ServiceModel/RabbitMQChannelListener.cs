using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RabbitMQ.Client;

namespace RabbitMQ.ServiceModel
{
    internal sealed class RabbitMQChannelListener<TChannel> : RabbitMQChannelListenerBase<IInputChannel> where TChannel : class, IChannel
    {

        private IInputChannel m_channel;
        private IModel m_model { get; set; }

        internal RabbitMQChannelListener(BindingContext context)
            : base(context)
        {
            m_channel = null;
            m_model = null;
        }

        protected override IInputChannel OnAcceptChannel(TimeSpan timeout)
        {
            // Since only one connection to a broker is required (even for communication
            // with multiple exchanges
            if (m_channel != null)
                return null;

            m_channel = new RabbitMQInputChannel(Context, m_model, new EndpointAddress(Uri.ToString()));
            m_channel.Closed += new EventHandler(ListenChannelClosed);
            return m_channel;
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return false;
        }

        protected override void OnOpen(TimeSpan timeout)
        {

#if VERBOSE
            DebugHelper.Start();
#endif
            m_model = m_bindingElement.Open(timeout);
#if VERBOSE
            DebugHelper.Stop(" ## In.Open {{Time={0}ms}}.");
#endif
        }

        protected override void OnClose(TimeSpan timeout)
        {
#if VERBOSE
            DebugHelper.Start();
#endif
            if (m_channel != null)
            {
                m_channel.Close();
                m_channel = null;
            }

            if (m_model != null)
            {
                m_bindingElement.Close(m_model, timeout);
                m_model = null;
            }
#if VERBOSE
            DebugHelper.Stop(" ## In.Close {{Time={0}ms}}.");
#endif
        }

        private void ListenChannelClosed(object sender, EventArgs args)
        {
            Close();
        }
    }
}
