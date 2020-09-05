using System;
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

        public AssaClient(string address, int port)
        {
            _serverAddress = IPAddress.Parse(address);
            _port = port;
        }
        public void Run()
        {
            TcpClient client = new TcpClient(_serverAddress.ToString(), _port);
            try
            {
                NetworkStream nwStream = client.GetStream();
                //ToDo: Figure the best way to send and recieve repeatedly (not to stop after one send);
                //ToDo: Test if the server can send data to the client, without recieving data from client first
                while (true)
                {

                    string textToSend = "a";
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

                    //---send the text---
                    Console.WriteLine("Sending : " + textToSend);
                    nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                    //---read back the text---
                    byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));

                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                //ToDo: Figure out a way to terminate client
                client.Close();
            }
        }
    }
}
