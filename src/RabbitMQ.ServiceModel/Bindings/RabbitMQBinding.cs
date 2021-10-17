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

using System.Configuration;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    /// <summary>
    /// A windows communication foundation binding over AMQP
    /// </summary>
    /// <remarks>Mode by Lee, remove ReliableSession</remarks>
    public sealed class RabbitMQBinding : Binding
    {
        #region Fields and Constructors

        public const long DefaultMaxMessageSize = 8192L;
        private bool _isInitialized;
        private TextMessageEncodingBindingElement _encoding;
        private RabbitMQTransportBindingElement _transport;
        private TransactionFlowBindingElement _transactionFlow;
        private CompositeDuplexBindingElement _compositeDuplex;

        /// <summary>
        /// Creates a new instance of the RabbitMQBinding class initialized
        /// to use the Protocols.DefaultProtocol. The broker must be set
        /// before use.
        /// </summary>
        public RabbitMQBinding()
        {
            base.Name = "RabbitMQBinding";
            base.Namespace = "http://schemas.rabbitmq.com/2007/RabbitMQ/";

            this.Initialize();
            this.TransactionFlow = true;
        }

        /// <summary>
        /// Uses the broker and protocol specified
        /// </summary>
        /// <param name="hostname">The hostname of the broker to connect to</param>
        /// <param name="port">The port of the broker to connect to</param>
        /// <param name="protocol">The protocol version to use</param>
        public RabbitMQBinding(string hostname, int port)
            : this()
        {
            this.HostName = hostname;
            this.Port = port;
        }

        /// <summary>
        /// Uses the broker, login and protocol specified
        /// </summary>
        /// <param name="hostname">The hostname of the broker to connect to</param>
        /// <param name="port">The port of the broker to connect to</param>
        /// <param name="username">The broker username to connect with</param>
        /// <param name="password">The broker password to connect with</param>
        /// <param name="virtualhost">The broker virtual host</param>
        /// <param name="maxMessageSize">The largest allowable encoded message size</param>
        public RabbitMQBinding(string hostname, int port, string username, string password, string virtualhost, long maxMessageSize)
            : this()
        {
            this.HostName = hostname;
            this.Port = port;
            this.Transport.Username = username;
            this.Transport.Password = password;
            this.Transport.VirtualHost = virtualhost;
            this.MaxMessageSize = maxMessageSize;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the scheme used by the binding, soap.amqp
        /// </summary>
        public override string Scheme
        {
            get { return CurrentVersion.Scheme; }
        }

        /// <summary>
        /// Gets the AMQP transport binding element
        /// </summary>
        public RabbitMQTransportBindingElement Transport
        {
            get { return this._transport; }
        }

        /// <summary>
        /// Determines whether or not the TransactionFlowBindingElement will
        /// be added to the channel stack
        /// </summary>
        public bool TransactionFlow { get; set; }

        /// <summary>
        /// Specifies whether or not the CompositeDuplex and ReliableSession
        /// binding elements are added to the channel stack.
        /// </summary>
        public bool OneWayOnly { get; set; }

        /// <summary>
        /// Specifies the hostname of the RabbitMQ Server
        /// </summary>
        [ConfigurationProperty("hostname")]
        public string HostName { get; set; }

        /// <summary>
        /// Specifies the RabbitMQ Server port
        /// </summary>
        [ConfigurationProperty("port")]
        public int Port { get; set; }

        /// <summary>
        /// Specifies the maximum encoded message size
        /// </summary>
        [ConfigurationProperty("maxmessagesize")]
        public long MaxMessageSize { get; set; }

        #endregion

        #region Methods

        public override BindingElementCollection CreateBindingElements()
        {
            this._transport.HostName = this.HostName;
            this._transport.Port = this.Port;
            if (this.MaxMessageSize != DefaultMaxMessageSize)
            {
                this._transport.MaxReceivedMessageSize = this.MaxMessageSize;
            }
            BindingElementCollection elements = new BindingElementCollection();

            if (this.TransactionFlow)
            {
                elements.Add(this._transactionFlow);
            }
            if (!this.OneWayOnly)
            {
                elements.Add(this._compositeDuplex);
            }
            elements.Add(this._encoding);
            elements.Add(this._transport);

            return elements;
        }

        private void Initialize()
        {
            lock (this)
            {
                if (!this._isInitialized)
                {
                    this._isInitialized = true;
                    this._transport = new RabbitMQTransportBindingElement();
                    this._encoding = new TextMessageEncodingBindingElement();
                    this._compositeDuplex = new CompositeDuplexBindingElement();
                    this._transactionFlow = new TransactionFlowBindingElement();
                    this.MaxMessageSize = DefaultMaxMessageSize;
                }
            }
        }

        #endregion
    }
}
