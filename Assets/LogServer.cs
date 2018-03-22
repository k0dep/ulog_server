using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Logging
{
    public class LogServer
    {
        public static LogServer Instance;
        public static readonly int DEFAULT_PORT = 56223;
        public static IPAddress multicastGroup = IPAddress.Parse("127.0.0.1");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            var port = DEFAULT_PORT;
            if (Environment.GetCommandLineArgs().Contains("--log-server-port"))
            {
                var portRaw = Environment
                    .GetCommandLineArgs()
                    .SkipWhile(t => t != "--log-server-port")
                    .Skip(1)
                    .FirstOrDefault();

                int portValue;
                if(portRaw != null)
                    if (int.TryParse(portRaw, out portValue))
                        port = portValue;
            }

            new LogServer(port);
        }




        public int Port { get; private set; }


        private UdpClient _client;
        private static DateTime unixDateTime = new DateTime(1970, 1, 1);
        private static uint id = 0;


        public LogServer(int port)
        {
            Instance = this;

            Port = port;
            _client = new UdpClient();
            _client.Connect(new IPEndPoint(multicastGroup, port));


            var helloBytes = Encoding.UTF8.GetBytes(formatMessage(LogType.Log, "application start", ""));
            _client.Send(helloBytes, helloBytes.Length);

            Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
        }


        ~LogServer()
        {
            var helloBytes = Encoding.UTF8.GetBytes(formatMessage(LogType.Log, "application exit", ""));
            _client.Send(helloBytes, helloBytes.Length);
            _client.Close();
        }


        private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            lock (_client)
            {
                var bytes = Encoding.UTF8.GetBytes(formatMessage(type, condition, stackTrace));

                _client.Send(bytes, bytes.Length);
            }
        }

        private static string formatMessage(LogType type, string condition, string stackTrace)
        {
            return JsonUtility.ToJson(new Message(id++, (ulong) ((DateTime.UtcNow - unixDateTime).TotalSeconds * 1000),
                type, condition, stackTrace));
        }

        class Message
        {
            public uint id;
            public ulong time;
            public LogType type;
            public string message;
            public string stacktrace;

            public Message(uint id, ulong time, LogType type, string message, string stacktrace)
            {
                this.id = id;
                this.time = time;
                this.type = type;
                this.message = message;
                this.stacktrace = stacktrace;
            }
        }
    }
}