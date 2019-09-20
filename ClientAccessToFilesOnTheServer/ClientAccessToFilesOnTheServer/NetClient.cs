using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientAccessToFilesOnTheServer
{
    class NetClient
    {
        public NetClient()
        {
        }

        private const string ip2 = "192.168.0.105";
        private const int port = 2048;
        private byte[] date;
        Socket tcpSocket;

        private byte[] buffer;
        private int size;
        private StringBuilder answer;
        bool flagToRedact;

        public void Client()
        {
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip2), port);
            using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                tcpSocket.Connect(tcpEndPoint);
                while (true)
                {
                    flagToRedact = true;
                    AnswerServer();
                    do
                    {
                        size = tcpSocket.Receive(buffer);
                        answer.Append(Encoding.ASCII.GetString(buffer, 0, size));
                    } while (tcpSocket.Available > 0);

                    NeedRedacting();
                    if (flagToRedact)
                    {
                        RedactFile();
                        continue;
                    }
                    Console.WriteLine(answer.ToString());
                    var message = Console.ReadLine();
                    SendMessage(message);
                    if (message == "Q")
                    {
                        break;
                    }
                }
            }
        }
        private void NeedRedacting()
        {
            for (int i = 0; i < 14; i++)
            {
                if (answer[i] == 'r')
                {
                    continue;
                }
                flagToRedact = false;
                break;
            }
        }
        private void RedactFile()
        {
            answer.Remove(0, 14);
            FileRedactClass fileStreamClass = new FileRedactClass(answer.ToString());
            var (savingFileFlag, file) = fileStreamClass.RedactFile();
            if (savingFileFlag)
            {
                SendMessage(file);
            }
            else
            {
                SendMessage("rrrrrrrrrrrrrr");
            }
        }
        private void AnswerServer()
        {
            buffer = new byte[256];
            size = 0;
            answer = new StringBuilder();
        }
        private void SendMessage(string message)
        {
            while (true)
            {
                date = Encoding.ASCII.GetBytes(message);
                if (date.Length == 0)
                {
                    message = Console.ReadLine();
                    continue;
                }
                break;
            }
            tcpSocket.Send(date);
        }
    }
}
