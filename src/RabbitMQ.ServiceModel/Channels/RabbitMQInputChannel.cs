#region License

// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2016 Pivotal Software, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License. 

#endregion

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RabbitMQInputChannel : RabbitMQInputChannelBase
    {
        #region Fields and Constructors

        private readonly RabbitMQTransportBindingElement _bindingElement;
        private readonly IModel _model;
        private readonly BlockingCollection<BasicDeliverEventArgs> _queue;
        private readonly MessageEncoder _encoder;
        private EventingBasicConsumer _consumer;

        public RabbitMQInputChannel(BindingContext bindingContext, IModel model, EndpointAddress address)
            : base(bindingContext, address)
        {
            this._bindingElement = bindingContext.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            this._model = model;
            this._queue = new BlockingCollection<BasicDeliverEventArgs>(new ConcurrentQueue<BasicDeliverEventArgs>());

            TextMessageEncodingBindingElement encoderElement = bindingContext.BindingParameters.Find<TextMessageEncodingBindingElement>();
            if (encoderElement != null)
            {
                encoderElement.ReaderQuotas.MaxStringContentLength = (int)this._bindingElement.MaxReceivedMessageSize;
                MessageEncoderFactory encoderFactory = encoderElement.CreateMessageEncoderFactory();
                this._encoder = encoderFactory.Encoder;
            }

            this._consumer = null;
        }

        #endregion

        #region Methods

        public override void Open(TimeSpan timeout)
        {
            if (base.State != CommunicationState.Created && base.State != CommunicationState.Closed)
            {
                throw new InvalidOperationException($"Cannot open the channel from the {base.State} state.");
            }

            this.OnOpening();
#if VERBOSE
            DebugHelper.Start();
#endif

            QueueDeclareOk queue = this._model.QueueDeclare(base.LocalAddress.Uri.PathAndQuery, true, false, true, null);
            this._consumer = new EventingBasicConsumer(this._model);
            this._consumer.Received += (sender, args) => this._queue.Add(args);
            this._model.BasicConsume(queue, false, this._consumer);

#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Open {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            base.OnOpened();
        }

        public override Message Receive(TimeSpan timeout) //TODO: timeout isn't used
        {
            try
            {
                BasicDeliverEventArgs message = this._queue.Take();
#if VERBOSE
                DebugHelper.Start();
#endif
                //TODO RabbitMQ.Cient 6.2.2 turn message.Body type byte[] to ReadOnlyMemory<byte>
                MemoryStream stream = new MemoryStream(message.Body.ToArray());
                Message result = this._encoder.ReadMessage(stream, (int)this._bindingElement.MaxReceivedMessageSize);
                result.Headers.To = base.LocalAddress.Uri;
                this._consumer.Model.BasicAck(message.DeliveryTag, false);
#if VERBOSE
                DebugHelper.Stop(" #### Message.Receive {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                        message.Body.Length,
                        result.Headers.Action.Remove(0, result.Headers.Action.LastIndexOf('/')));
#endif
                return result;
            }
            catch (EndOfStreamException)
            {
                if (this._consumer == null || (this._consumer.ShutdownReason != null && this._consumer.ShutdownReason.ReplyCode != Constants.ReplySuccess))
                {
                    this.OnFaulted();
                }

                this.Close();

                return null;
            }
            catch (XmlException)
            {
                this.OnFaulted();
                this.Close();

                return null;
            }
        }

        public override bool TryReceive(TimeSpan timeout, out Message message)
        {
            message = this.Receive(timeout);

            return true;
        }

        public override bool WaitForMessage(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public override void Close(TimeSpan timeout)
        {
            if (base.State == CommunicationState.Closed || base.State == CommunicationState.Closing)
            {
                return; // Ignore the call, we're already closing.
            }

            this.OnClosing();
#if VERBOSE
            DebugHelper.Start();
#endif
            if (this._consumer != null)
            {
                //TODO RabbitMQ.Cient 6.2.2 turn ConsumerTag to ConsumerTags
                foreach (string consumerTag in this._consumer.ConsumerTags)
                {
                    this._model.BasicCancel(consumerTag);
                }

                this._consumer = null;
            }
#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Close {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            this.OnClosed();
        }

        #endregion
    }
}
