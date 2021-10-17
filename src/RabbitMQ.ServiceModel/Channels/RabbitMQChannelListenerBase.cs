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

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace RabbitMQ.ServiceModel
{
    internal abstract class RabbitMQChannelListenerBase<TChannel> : ChannelListenerBase<TChannel>
        where TChannel : class, IChannel
    {
        #region Fields and Constructors

        private readonly Func<TimeSpan, TChannel> _acceptChannelMethod;
        private readonly Func<TimeSpan, bool> _waitForChannelMethod;
        private readonly Action<TimeSpan> _openMethod;
        private readonly Action<TimeSpan> _closeMethod;
        private readonly BindingContext _bindingContext;
        private readonly Uri _listenUri;
        protected RabbitMQTransportBindingElement _bindingElement;

        protected RabbitMQChannelListenerBase(BindingContext bindingContext)
        {
            this._acceptChannelMethod = this.OnAcceptChannel;
            this._waitForChannelMethod = this.OnWaitForChannel;
            this._openMethod = this.OnOpen;
            this._closeMethod = this.OnClose;
            this._bindingContext = bindingContext;
            this._bindingElement = bindingContext.Binding.Elements.Find<RabbitMQTransportBindingElement>();

            if (bindingContext.ListenUriMode == ListenUriMode.Explicit && bindingContext.ListenUriBaseAddress != null)
            {
                this._listenUri = new Uri(bindingContext.ListenUriBaseAddress, bindingContext.ListenUriRelativeAddress);
            }
            else
            {
                Uri baseUri = new Uri("soap.amqp:///");
                this._listenUri = new Uri(baseUri, Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Properties

        public override Uri Uri
        {
            get { return this._listenUri; }
        }

        protected BindingContext BindingContext
        {
            get { return this._bindingContext; }
        }

        #endregion

        #region Methods

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._acceptChannelMethod.BeginInvoke(timeout, callback, state);
        }

        protected override TChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return this._acceptChannelMethod.EndInvoke(result);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._waitForChannelMethod.BeginInvoke(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this._waitForChannelMethod.EndInvoke(result);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._openMethod.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this._openMethod.EndInvoke(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this._closeMethod.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this._closeMethod.EndInvoke(result);
        }

        protected override void OnAbort()
        {
            this.OnClose(this._bindingContext.Binding.CloseTimeout);
        }

        #endregion
    }
}
