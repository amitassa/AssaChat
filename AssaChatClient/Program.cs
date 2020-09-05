using System;

namespace AssaChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write your client number:");
            int number = int.Parse(Console.ReadLine());
            AssaClient client = new AssaClient("127.0.0.1",8844, number);
            client.Run();

        }
    }
}
