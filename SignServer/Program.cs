using System;
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
            string pathToPython = "C:\\Program Files\\Python38\\python.exe";
            string pathToSkript = "D:\\Soft_DSA\\neural.py";
            Neural.Init(pathToPython, pathToSkript);

            server = new Server(56565, "D:\\Soft_DSA\\Soft_DSA.cer");

            Console.WriteLine("Press any key to stop server");
            Console.ReadKey();

            server.StopServer();
            Neural.Close();
        }
    }
}
