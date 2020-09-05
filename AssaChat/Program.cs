using System;

namespace AssaChat
{
    class Program
    {
        static void Main(string[] args)
        {
            AssaServer server = new AssaServer(8844);
            server.Run();
        }
    }
}
