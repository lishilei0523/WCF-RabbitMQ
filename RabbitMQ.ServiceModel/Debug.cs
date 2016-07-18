using System;
using System.Diagnostics;

namespace RabbitMQ.ServiceModel
{
    public static class DebugHelper
    {
        static long m_started;

        static DebugHelper()
        {
            Timer = new Stopwatch();
        }

        public static void Start()
        {
            m_started = Timer.ElapsedMilliseconds;
            Timer.Start();
        }

        public static void Stop(string messageFormat, params object[] args)
        {
            Timer.Stop();

            if (Enabled)
            {
                object[] args1 = new object[args.Length + 1];
                args.CopyTo(args1, 1);
                args1[0] = Timer.ElapsedMilliseconds - m_started;
                if (Console.CursorLeft != 0)
                    Console.WriteLine();
                Console.WriteLine(messageFormat, args1);
            }
        }

        private static Stopwatch timer;
        public static Stopwatch Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        private static bool enabled;
        public static bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}
