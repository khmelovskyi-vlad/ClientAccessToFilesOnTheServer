using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientAccessToFilesOnTheServer
{
    class Connection
    {
        public Connection()
        {
        }

        private const string ip2 = "192.168.0.106";
        private const int port = 2048;
        private Socket tcpSocket;


        public void Client()
        {
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip2), port);
            using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    tcpSocket.Connect(tcpEndPoint);
                    ClientManager clientManager = new ClientManager(tcpSocket);
                    clientManager.FindNeedFiles();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }
    }
}
