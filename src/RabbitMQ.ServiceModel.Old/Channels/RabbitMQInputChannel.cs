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
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RabbitMQInputChannel : RabbitMQInputChannelBase
    {
        private static readonly object _Sync = new object();
        private RabbitMQTransportBindingElement m_bindingElement;
        private MessageEncoder m_encoder;
        private IModel m_model;
        private EventingBasicConsumer m_consumer;
        private BlockingCollection<BasicDeliverEventArgs> m_queue =
            new BlockingCollection<BasicDeliverEventArgs>(new ConcurrentQueue<BasicDeliverEventArgs>());

        public RabbitMQInputChannel(BindingContext context, IModel model, EndpointAddress address)
            : base(context, address)
        {
            m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            TextMessageEncodingBindingElement encoderElem = context.BindingParameters.Find<TextMessageEncodingBindingElement>();
            encoderElem.ReaderQuotas.MaxStringContentLength = (int)m_bindingElement.MaxReceivedMessageSize;
            if (encoderElem != null)
            {
                m_encoder = encoderElem.CreateMessageEncoderFactory().Encoder;
            }
            m_model = model;
            m_consumer = null;
        }


        public override Message Receive(TimeSpan timeout) //TODO: timeout isn't used
        {
            try
            {
                BasicDeliverEventArgs msg = m_queue.Take();
#if VERBOSE
                DebugHelper.Start();
#endif
                //TODO RabbitMQ.Cient 6.2.2 turn msg.Body type byte[] to ReadOnlyMemory<byte>
                Message result = m_encoder.ReadMessage(new MemoryStream(msg.Body.ToArray()), (int)m_bindingElement.MaxReceivedMessageSize);
                result.Headers.To = base.LocalAddress.Uri;
                m_consumer.Model.BasicAck(msg.DeliveryTag, false);
#if VERBOSE
                DebugHelper.Stop(" #### Message.Receive {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                        msg.Body.Length,
                        result.Headers.Action.Remove(0, result.Headers.Action.LastIndexOf('/')));
#endif
                return result;
            }
            catch (EndOfStreamException)
            {
                if (m_consumer == null || m_consumer.ShutdownReason != null && m_consumer.ShutdownReason.ReplyCode != Constants.ReplySuccess)
                {
                    OnFaulted();
                }
                Close();
                return null;
            }
        }

        public override bool TryReceive(TimeSpan timeout, out Message message)
        {
            message = Receive(timeout);
            return true;
        }

        public override bool WaitForMessage(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public override void Close(TimeSpan timeout)
        {

            if (base.State == CommunicationState.Closed
                || base.State == CommunicationState.Closing)
            {
                return; // Ignore the call, we're already closing.
            }

            OnClosing();
#if VERBOSE
            DebugHelper.Start();
#endif
            if (m_consumer != null)
            {
                //TODO RabbitMQ.Cient 6.2.2 turn ConsumerTag to ConsumerTags
                foreach (string consumerTag in m_consumer.ConsumerTags)
                {
                    m_model.BasicCancel(consumerTag);
                }
                m_consumer = null;
            }
#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Close {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            OnClosed();
        }

        public override void Open(TimeSpan timeout)
        {
            if (State != CommunicationState.Created && State != CommunicationState.Closed)
                throw new InvalidOperationException(string.Format("Cannot open the channel from the {0} state.", base.State));

            OnOpening();
#if VERBOSE
            DebugHelper.Start();
#endif
            //TODO Powered by Lee, add lock
            lock (_Sync)
            {
                QueueDeclareOk queue = m_model.QueueDeclare(base.LocalAddress.Uri.PathAndQuery, true, false, true, null);
                m_consumer = new EventingBasicConsumer(m_model);
                m_consumer.Received += (sender, args) => m_queue.Add(args);
                m_model.BasicConsume(queue, false, m_consumer);
            }

#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Open {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            OnOpened();
        }
    }
}
