﻿using System;
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
        private Socket tcpSocket;

        private byte[] buffer;
        private int size;
        private StringBuilder answer;
        private bool flagToRedact;

        public void Client()
        {
            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip2), port);
            using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    tcpSocket.Connect(tcpEndPoint);
                    while (true)
                    {
                        flagToRedact = true;
                        AnswerServer();
                        NeedRedacting();
                        if (flagToRedact)
                        {
                            RedactFile();
                            continue;
                        }
                        Console.WriteLine(answer.ToString());
                        var message = WriteMessage();
                        if (message == "Q")
                        {
                            break;
                        }
                        SendMessage(message);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }
        private string WriteMessage()
        {
            StringBuilder message = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    return message.ToString();
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    throw new OperationCanceledException();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    try
                    {
                        message.Remove(message.Length - 1, 1);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        continue;
                    }
                }
                else
                {
                    message.Append(key.KeyChar);
                }
            }
        }
        private void NeedRedacting()
        {
            for (int i = 0; i < 3; i++)
            {
                if (answer[i] == '?')
                {
                    continue;
                }
                flagToRedact = false;
                break;
            }
        }
        private void RedactFile()
        {
            try
            {
                answer.Remove(0, 3);
                FileRedactClass fileStreamClass = new FileRedactClass(answer.ToString());
                var (savingFileFlag, file) = fileStreamClass.RedactFile();
                if (savingFileFlag)
                {
                    SendMessage(file);
                }
                else
                {
                    SendMessage("???");
                }
            }
            catch (SocketException)
            {

                throw;
            }
        }
        private void AnswerServer()
        {
            buffer = new byte[256];
            size = 0;
            answer = new StringBuilder();
            do
            {
                size = tcpSocket.Receive(buffer);
                answer.Append(Encoding.ASCII.GetString(buffer, 0, size));
            } while (tcpSocket.Available > 0);
        }
        private void SendMessage(string message)
        {
            while (true)
            {
                date = Encoding.ASCII.GetBytes(message);
                if (date.Length == 0)
                {
                    message = WriteMessage();
                    continue;
                }
                break;
            }
            tcpSocket.Send(date);
        }
    }
}
