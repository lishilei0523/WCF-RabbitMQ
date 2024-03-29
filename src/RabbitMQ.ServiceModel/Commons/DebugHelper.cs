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
using System.Diagnostics;

namespace RabbitMQ.ServiceModel
{
    public static class DebugHelper
    {
        #region Fields and Constructors

        private static readonly Stopwatch _Timer;

        private static long _Started;

        static DebugHelper()
        {
            _Timer = new Stopwatch();
        }

        #endregion

        #region Methods

        public static void Start()
        {
            _Started = _Timer.ElapsedMilliseconds;
            _Timer.Start();
        }

        public static void Stop(string messageFormat, params object[] parameters)
        {
            _Timer.Stop();

            object[] arguments = new object[parameters.Length + 1];
            parameters.CopyTo(arguments, 1);
            arguments[0] = _Timer.ElapsedMilliseconds - _Started;

            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }

            Console.WriteLine(messageFormat, arguments);
        }

        #endregion
    }
}
