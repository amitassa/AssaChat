using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AssaChat
{
    public class AssaServer
    {
        private IPAddress _localAddress;
        private TcpListener _server;
        private ConcurrentDictionary<TcpClient, string> _clientsList;

        public AssaServer(int port)
        {
            _localAddress = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(_localAddress, port);
            _clientsList = new ConcurrentDictionary<TcpClient, string>();
        }
        public void Run()
        {

            _server.Start();
            Console.WriteLine($"Listening at {_server.LocalEndpoint}. Waiting for connections.");

            try
            {
                // ToDo: Figure a way to accept client connections async at the best way.
                while (true)
                {
                    //---incoming client connected---
                    TcpClient client = _server.AcceptTcpClient();

                    Console.WriteLine("Connected to: {0}:{1} ",
                        ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
                        ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString());
                    

                    object obj = new object();
                    ThreadPool.QueueUserWorkItem(obj =>
                    {
                        addClientToList(((IPEndPoint)client.Client.RemoteEndPoint).Port, client);
                        //---get the incoming data through a network stream---
                        NetworkStream nwStream = client.GetStream();
                        byte[] buffer = new byte[client.ReceiveBufferSize];

                        string dataReceived;

                        do
                        {
                            //---read incoming stream---
                            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                            //---convert the data received into a string---
                            dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine("Received : " + dataReceived);

                            //---write back the text to the client---
                            Console.WriteLine("Sending back : " + dataReceived);
                            SendToAllClients(buffer, bytesRead);
                        }
                        while (dataReceived.ToLower() != "exit");
                        //ToDo: to send & recieve repeatedly, should find a way to loop the send & receive 
                        //      and take the client.close() out of the loop
                        client.Close();
                    }, null);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                Console.WriteLine("Terminating...");
                _server.Stop();
            }

        }
        private void addClientToList(int port, TcpClient client)
        {

            _clientsList.TryAdd(client, port.ToString());

        }
        private void SendToAllClients(byte[] buffer, int bytesRead)
        {
            foreach (var client_port in _clientsList)
            {
                NetworkStream nwStream = client_port.Key.GetStream();
                nwStream.Write(buffer, 0, bytesRead);
            }
        }


    }
}
