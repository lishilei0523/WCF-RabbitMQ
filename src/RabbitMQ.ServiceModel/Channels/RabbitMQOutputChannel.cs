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
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    internal sealed class RabbitMQOutputChannel : RabbitMQOutputChannelBase
    {
        #region Fields and Constructors

        private readonly RabbitMQTransportBindingElement _bindingElement;
        private readonly IModel _model;
        private readonly MessageEncoder _encoder;

        public RabbitMQOutputChannel(BindingContext bindingContext, IModel model, EndpointAddress endpointAddress)
            : base(bindingContext, endpointAddress)
        {
            this._bindingElement = bindingContext.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            this._model = model;

            MessageEncodingBindingElement encoderElement = bindingContext.Binding.Elements.Find<MessageEncodingBindingElement>();
            if (encoderElement != null)
            {
                MessageEncoderFactory encoderFactory = encoderElement.CreateMessageEncoderFactory();
                this._encoder = encoderFactory.Encoder;
            }
        }

        #endregion

        #region Methods

        public override void Open(TimeSpan timeout)
        {
            if (base.State != CommunicationState.Created && base.State != CommunicationState.Closed)
            {
                throw new InvalidOperationException($"Cannot open the channel from the {base.State} state.");
            }

            base.OnOpening();
            base.OnOpened();
        }

        public override void Send(Message message, TimeSpan timeout)
        {
            if (message.State != MessageState.Closed)
            {
#if DEBUG
                DebugHelper.Start();
#endif
                byte[] body;
                using (MemoryStream stream = new MemoryStream())
                {
                    this._encoder.WriteMessage(message, stream);
                    body = stream.ToArray();
                }
#if DEBUG
                DebugHelper.Stop(" #### Message.Send {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                    body.Length,
                    message.Headers.Action.Remove(0, message.Headers.Action.LastIndexOf('/')));
#endif
                this._model.BasicPublish(string.Empty, base.RemoteAddress.Uri.PathAndQuery, null, body);
            }
        }

        public override void Close(TimeSpan timeout)
        {
            if (base.State == CommunicationState.Closed || base.State == CommunicationState.Closing)
            {
                // Ignore the call, we're already closing.
                return;
            }

            this.OnClosing();
            this.OnClosed();
        }

        #endregion
    }
}
