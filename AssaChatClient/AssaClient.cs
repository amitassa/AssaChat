﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AssaChatClient
{
    public class AssaClient
    {
        private IPAddress _serverAddress;
        private int _port;
        private TcpClient _client;
        public string Name;

        public AssaClient(string address, int port, int clientNumber)
        {
            _serverAddress = IPAddress.Parse(address);
            _port = port;
            Name = "client" + clientNumber;
            _client = new TcpClient(_serverAddress.ToString(), _port);
        }
        public void Run()
        {
            try
            {
                using (NetworkStream nwStream = _client.GetStream())
                {
                    ThreadPool.QueueUserWorkItem(obj => ReceiveMessages(nwStream));
                    while (true)
                    {
                        WriteAMessage(nwStream);   
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _client.Close();
            }
        }

        private void WriteAMessage(NetworkStream nwStream)
        {
            Console.WriteLine("Your message:");
            string msg = Console.ReadLine();
            string textToSend = $"{Name}: {msg}";
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

            //---send the text---
            Console.WriteLine("Sending : " + msg);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
        private void ReceiveMessages(NetworkStream nwStream)
        {
            //---read back the text---
            while (true)
            {
                byte[] bytesToRead = new byte[_client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);
                Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            }
        }
    }
}
