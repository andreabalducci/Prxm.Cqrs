using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace Sample.DebugUi.Infrastructure
{
    public class UdpInterceptor
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
        private void Start(Object state)
        {
            while (true)
            {
                if (_stop)
                    break;
                try
                {
                    byte[] sent = _udpClient.Receive(ref _ipe);

                }
                catch (SocketException e)
                {

                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
