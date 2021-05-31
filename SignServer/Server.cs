using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignServer
{
    class Server
    {
        private TcpListener Listener;
        private X509Certificate serverCertificate = null;
        private ushort port;
        private Mutex mutex = new Mutex();
        private Thread handle, clients_cleaner;
        private bool server_alive = true;
        public List<Client> clients;

        public Server(ushort Port, string certFile)
        {
            port = Port;
            clients = new List<Client>();
            Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();
            Console.WriteLine("Server started at port " + port.ToString());
            serverCertificate = X509Certificate.CreateFromCertFile(certFile);

            handle = new Thread(ClientsHandler);
            clients_cleaner = new Thread(CleanDead);
            handle.Start();
            clients_cleaner.Start();
        }

        private void ClientsHandler()
        {
            bool mtx = false;
            try
            {
                while (server_alive)
                {
                    TcpClient newClient = Listener.AcceptTcpClient();
                    mutex.WaitOne();
                    mtx = true;
                    clients.Add(new Client(newClient, serverCertificate));
                    mutex.ReleaseMutex();
                    mtx = false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error, when accept new client");
                if (mtx) mutex.ReleaseMutex();
            }
        }

        private void CleanDead()
        {
            while (server_alive)
            {
                try
                {
                    mutex.WaitOne();
                    clients.RemoveAll(client => !client.alive);
                    mutex.ReleaseMutex();
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Error when remove dead client: " + exp.Message);
                    mutex.ReleaseMutex();
                }

                Thread.Sleep(1000);
            }
        }

        public void StopServer()
        {
            server_alive = false;
            if (Listener != null)
            {
                Listener.Stop();
            }

            handle.Join();
            clients_cleaner.Join();

            foreach (Client client in clients)
            {
                client.sslStream.Close();
            }
            Console.WriteLine("Server stopped");
        }
    }
}
