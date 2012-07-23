using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using Castle.Core;

namespace Sample.DebugUi.Infrastructure
{
    public class UdpInterceptor : ILogInterceptor, IStartable
    {
        private UdpClient _udpClient;
        private Boolean _stop;
        private IPEndPoint _ipe;

        public UdpInterceptor()
        {
            try
            {
                _udpClient = new UdpClient(8080);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error when opening socked: " + ex.ToString());
            }

            Sleeping = false;

        }

        public bool Sleeping { get; set; }

        public void Close()
        {
            _udpClient.Close();
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(Start, null);
        }

        public void Stop()
        {
            _stop = true;
        }

        private void Start(Object state)
        {
            while (true)
            {
                if (_stop)
                    break;
                try
                {
                    byte[] sent = _udpClient.Receive(ref _ipe);
                    String stringValue = Encoding.UTF8.GetString(sent);
                    XElement element = XElement.Parse(stringValue);
                    LogMessage message = new LogMessage();
                    message.Logger = element.Attribute("logger").Value;
                    message.Timestamp = DateTime.Parse(element.Attribute("timestamp").Value);
                    message.Level = element.Attribute("level").Value;
                    switch (message.Level) { 
                        case "ERROR":
                            message.NumericLevel = 4;
                            break;
                        case "WARN":
                            message.NumericLevel = 3;
                            break;
                        case "INFO":
                            message.NumericLevel = 2;
                            break;
                        case "DEBUG":
                            message.NumericLevel = 1;
                            break;

                    }
                    message.ThreadId = element.Attribute("thread").Value;
                    message.Message = (String)element.Element("message");
                    var exceptionNode = element.Element("exception");
                    if (exceptionNode != null) {
                        message.Exception = (String) exceptionNode;
                    }
                    foreach (var dataNode in element.Descendants("data"))
                    {
                        String key = dataNode.Attribute("name").Value;
                        String value = dataNode.Attribute("value").Value;
                        message.Properties.Add(key, value);
                        switch (key)
                        {
                            case "op_type":
                                message.OpType = value;
                            break;
                            case "op_type_id":
                                message.OpTypeId = value;
                            break;
                        }
                    }
                    OnLogIntercepted(message);
                }
                catch (SocketException e)
                {

                }
                catch (Exception e)
                {

                }
            }
        }

        public event EventHandler<LogInterceptedEventArgs> LogIntercepted;

        protected void OnLogIntercepted(LogMessage message)
        {
            var temp = LogIntercepted;
            if (temp != null)
            {

                LogInterceptedEventArgs args = new LogInterceptedEventArgs(message);
                temp(this, args);
            }
        }
    }
}
