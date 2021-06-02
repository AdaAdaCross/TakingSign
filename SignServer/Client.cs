using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;

namespace SignServer
{
    class Client
    {
        public bool alive;

        public SslStream sslStream;
        private string id = "0";
        private string hash_id = "0";
        private Mutex mutex;
        private Thread clientThread;
        private Random rnd = new Random();

        public Client(TcpClient client, X509Certificate serverCertificate)
        {
            mutex = new Mutex();
            sslStream = new SslStream(client.GetStream(), false);

            try
            {
                sslStream.AuthenticateAsServer(serverCertificate,
                    clientCertificateRequired: false,
                    checkCertificateRevocation: true);

                //sslStream.ReadTimeout = -1;
                //sslStream.WriteTimeout = 5000;

                Console.WriteLine("New client from " +
                    IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString())
                    + " connected at port " +
                    ((IPEndPoint)client.Client.RemoteEndPoint).Port);

                id = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() + ":" +
                    ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
                hash_id = Math.Abs(id.GetHashCode()).ToString();

                alive = true;

                clientThread = new Thread(ClientHandler);
                clientThread.Start();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to add new client with error: " + exp.Message);
                sslStream.Close();
                client.Close();
                return;
            }
        }

        private void ClientHandler()
        {
            while (alive)
            {
                byte[] data = GetMessage();
                if (data == null)
                    return;

                byte code = 0;
                string message = "";
                ParseMessage(data, ref code, ref message);
                
                if (code == 1)
                {
                    string filename = "D:\\Soft_DSA\\" + hash_id + ".png";
                    Console.WriteLine("User '" + id + "' send sign");
                    int[][] sign = Worker.ReadFromString(message);
                    Worker.NormalizeSign(sign);
                    Worker.DrawTo(sign, filename);

                    int result = Neural.Auth(filename);
                    Console.WriteLine("User '" + id + "' received result: " + result);

                    byte[] answer = GenerateMessage(200, result.ToString());
                    SendMessage(answer);

                    if (File.Exists(filename))
                        File.Delete(filename);
                    continue;
                }
            }
        }

        private byte[] GetMessage()
        {
            int code;
            byte[] size = new byte[4];

            try
            {
                code = sslStream.Read(size, 0, 4);
                if (code == 0)
                    throw new Exception("User disconnected");
                if (code != 4)
                    throw new Exception("Wrong input message header");

                int buf_sz = BitConverter.ToInt32(size, 0);
                byte[] buffer = new byte[buf_sz];
                code = sslStream.Read(buffer, 0, buf_sz);
                if (code != buf_sz)
                    throw new Exception("Wrong input message");

                return buffer;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error at user '" + id + "' " + exp.Message);
                alive = false;
                return null;
            }
        }

        private bool SendMessage(byte[] message)
        {
            try
            {
                int mes_size = message.Length;
                byte[] row_size = BitConverter.GetBytes(mes_size);

                sslStream.Write(row_size);
                sslStream.Write(message);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error at user '" + id + "' " + exp.Message);
                alive = false;
                return false;
            }

            return true;
        }

        private byte[] GenerateMessage(byte code, string message)
        {
            byte[] row_message = Encoding.Unicode.GetBytes(message);
            byte[] full_message = new byte[3 + row_message.Length];
            full_message[0] = code;
            full_message[2 + row_message.Length] = 0;
            Array.Copy(row_message, 0, full_message, 2, row_message.Length);
            return full_message;
        }

        private void ParseMessage(byte[] row, ref byte code, ref string message)
        {
            code = row[0];
            message = Encoding.Unicode.GetString(row, 2, row.Length - 3);
        }
    }
}
