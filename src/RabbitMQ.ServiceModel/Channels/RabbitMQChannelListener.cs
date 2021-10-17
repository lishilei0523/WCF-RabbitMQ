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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    internal sealed class RabbitMQChannelListener<TChannel> : RabbitMQChannelListenerBase<IInputChannel> where TChannel : class, IChannel
    {
        #region Fields and Constructors

        private IInputChannel _channel;
        private IModel _model;

        internal RabbitMQChannelListener(BindingContext context)
            : base(context)
        {
            this._channel = null;
            this._model = null;
        }

        #endregion

        #region Methods

        protected override IInputChannel OnAcceptChannel(TimeSpan timeout)
        {
            // Since only one connection to a broker is required (even for communication
            // with multiple exchanges
            if (this._channel != null)
            {
                return null;
            }

            EndpointAddress endpointAddress = new EndpointAddress(base.Uri.ToString());
            this._channel = new RabbitMQInputChannel(base.BindingContext, this._model, endpointAddress);
            this._channel.Closed += this.ListenChannelClosed;
            return this._channel;
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
            this._model = this._bindingElement.Open(timeout);
#if VERBOSE
            DebugHelper.Stop(" ## In.Open {{Time={0}ms}}.");
#endif
        }

        protected override void OnClose(TimeSpan timeout)
        {
#if VERBOSE
            DebugHelper.Start();
#endif
            if (this._channel != null)
            {
                this._channel.Close();
                this._channel = null;
            }

            if (this._model != null)
            {
                base._bindingElement.Close(this._model, timeout);
                this._model = null;
            }
#if VERBOSE
            DebugHelper.Stop(" ## In.Close {{Time={0}ms}}.");
#endif
        }

        private void ListenChannelClosed(object sender, EventArgs args)
        {
            base.Close();
        }

        #endregion
    }
}
