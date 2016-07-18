namespace RabbitMQ.ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using Client;

    internal sealed class RabbitMQChannelFactory : ChannelFactoryBase<IOutputChannel>
    {
        private readonly BindingContext _context;
        private readonly Action<TimeSpan> _openMethod;
        private readonly RabbitMQTransportBindingElement _bindingElement;
        private IModel _model { get; set; }

        public RabbitMQChannelFactory(BindingContext context)
        {
            _context = context;
            _openMethod = Open;
            _bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            _model = null;
        }

        protected override IOutputChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            return new RabbitMQOutputChannel(_context, _model, address);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _openMethod.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _openMethod.EndInvoke(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
#if VERBOSE
            DebugHelper.Start();
#endif
            _model = _bindingElement.Open(timeout);
#if VERBOSE
            DebugHelper.Stop(" ## Out.Open {{Time={0}ms}}.");
#endif
        }

        protected override void OnClose(TimeSpan timeout)
        {
#if VERBOSE
            DebugHelper.Start();
#endif

            if (_model != null)
            {
                _bindingElement.Close(_model, timeout);
                _model = null;
            }

#if VERBOSE
            DebugHelper.Stop(" ## Out.Close {{Time={0}ms}}.");
#endif
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            OnClose(_context.Binding.CloseTimeout);
        }
    }
}
