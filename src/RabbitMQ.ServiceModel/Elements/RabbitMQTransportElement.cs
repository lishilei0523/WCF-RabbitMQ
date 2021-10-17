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
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace RabbitMQ.ServiceModel
{
    public sealed class RabbitMQTransportElement : TransportElement
    {
        #region Properties

        /// <summary>
        /// Specifies the hostname of the broker that the binding should connect to.
        /// </summary>
        [ConfigurationProperty("hostname", IsRequired = true)]
        public string HostName
        {
            get { return ((string)base["hostname"]); }
            set { base["hostname"] = value; }
        }

        /// <summary>
        /// Specifies the port of the broker that the binding should connect to.
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = AmqpTcpEndpoint.UseDefaultPort)]
        public int Port
        {
            get { return ((int)base["port"]); }
            set { base["port"] = value; }
        }

        /// <summary>
        /// The username to use when authenticating with the broker
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = ConnectionFactory.DefaultUser)]
        public string Username
        {
            get { return ((string)base["username"]); }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Password to use when authenticating with the broker
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = ConnectionFactory.DefaultPass)]
        public string Password
        {
            get { return ((string)base["password"]); }
            set { base["password"] = value; }
        }

        /// <summary>
        /// The virtual host to access.
        /// </summary>
        [ConfigurationProperty("virtualHost", DefaultValue = ConnectionFactory.DefaultVHost)]
        public string VirtualHost
        {
            get { return ((string)base["virtualHost"]); }
            set { base["virtualHost"] = value; }
        }

        public override Type BindingElementType
        {
            get { return typeof(RabbitMQTransportElement); }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection configurationProperties = base.Properties;
                Type currentType = this.GetType();
                PropertyInfo[] publicProperties = currentType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo propertyInfo in publicProperties)
                {
                    object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), false);
                    foreach (ConfigurationPropertyAttribute attribute in customAttributes)
                    {
                        configurationProperties.Add(new ConfigurationProperty(attribute.Name, propertyInfo.PropertyType, attribute.DefaultValue));
                    }
                }

                return configurationProperties;
            }
        }

        #endregion

        #region Methods

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            if (bindingElement == null)
            {
                throw new ArgumentNullException(nameof(bindingElement));
            }
            if (!(bindingElement is RabbitMQTransportBindingElement rabbitMqTransportBindingElement))
            {
                throw new ArgumentException($"Invalid type for binding. Expected {typeof(RabbitMQBinding).AssemblyQualifiedName}, Passed: {bindingElement.GetType().AssemblyQualifiedName}");
            }

            rabbitMqTransportBindingElement.HostName = this.HostName;
            rabbitMqTransportBindingElement.Port = this.Port;
            rabbitMqTransportBindingElement.ConnectionFactory.Password = this.Password;
            rabbitMqTransportBindingElement.ConnectionFactory.UserName = this.Username;
            rabbitMqTransportBindingElement.ConnectionFactory.VirtualHost = this.VirtualHost;
        }

        public override void CopyFrom(ServiceModelExtensionElement serviceModelExtensionElement)
        {
            base.CopyFrom(serviceModelExtensionElement);
            if (serviceModelExtensionElement is RabbitMQTransportElement rabbitMqTransportElement)
            {
                this.HostName = rabbitMqTransportElement.HostName;
                this.Port = rabbitMqTransportElement.Port;
                this.Password = rabbitMqTransportElement.Password;
                this.Username = rabbitMqTransportElement.Username;
                this.VirtualHost = rabbitMqTransportElement.VirtualHost;
            }
        }

        protected override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);

            if (bindingElement == null)
            {
                throw new ArgumentNullException(nameof(bindingElement));
            }
            if (!(bindingElement is RabbitMQTransportBindingElement rabbitMqTransportBindingElement))
            {
                throw new ArgumentException($"Invalid type for binding. Expected {typeof(RabbitMQBinding).AssemblyQualifiedName}, Passed: {bindingElement.GetType().AssemblyQualifiedName}");
            }

            this.HostName = rabbitMqTransportBindingElement.HostName;
            this.Port = rabbitMqTransportBindingElement.Port;
            this.Password = rabbitMqTransportBindingElement.ConnectionFactory.Password;
            this.Username = rabbitMqTransportBindingElement.ConnectionFactory.UserName;
            this.VirtualHost = rabbitMqTransportBindingElement.ConnectionFactory.VirtualHost;
        }

        protected override BindingElement CreateBindingElement()
        {
            TransportBindingElement transportBindingElement = this.CreateDefaultBindingElement();
            this.ApplyConfiguration(transportBindingElement);

            return transportBindingElement;
        }

        protected override TransportBindingElement CreateDefaultBindingElement()
        {
            return new RabbitMQTransportBindingElement();
        }

        #endregion
    }
}
