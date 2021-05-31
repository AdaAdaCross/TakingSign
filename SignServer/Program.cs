using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignServer
{
    class Program
    {
        static private Server server;

        static void Main(string[] args)
        {
            server = new Server(56565, "D:\\Soft_DSA\\Soft_DSA.cer");

            string csv_path = "D:\\Soft_DSA\\csv";
            string png_path = "D:\\Soft_DSA\\png";

            Console.WriteLine("Press any key to stop server");
            Console.ReadKey();

            server.StopServer();
        }
    }
}
