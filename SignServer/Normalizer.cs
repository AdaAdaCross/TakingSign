using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignServer
{
    public static class Worker
    {
        private const int req_size = 64;
        private const float widht = 2;

        public static void CreateImages(string csv_path, string png_path)
        {
            string[] dirs = Directory.GetDirectories(csv_path);
            foreach (string dir in dirs)
            {
                string png_dir = dir.Replace(csv_path, png_path);
                if (!Directory.Exists(png_dir))
                {
                    Directory.CreateDirectory(png_dir);
                }

                string[] files = Directory.GetFiles(dir, "*.csv");
                foreach (string file in files)
                {
                    string png_file = file.Replace(csv_path, png_path);
                    png_file = png_file.Replace(".csv", ".png");

                    int[][] sign = Worker.ReadFromCSV(file);
                    NormalizeSign(sign);
                    DrawTo(sign, png_file);
                }
            }
        }
        public static int[][] ReadFromString(string str_sign)
        {
            List<int[]> rows = new List<int[]>();

            string[] sign_rows = str_sign.Split(' ');
            foreach (string row in sign_rows)
            {
                if (row == "")
                    break;
                string[] numbers = row.Split(',');
                int X = Convert.ToInt32(numbers[0]);
                int Y = Convert.ToInt32(numbers[1]);
                int Press = Convert.ToInt32(numbers[2]);

                int[] current_params = { X, Y, Press };
                rows.Add(current_params);
            }

            return rows.ToArray();
        }
        public static int[][] ReadFromCSV(string filename)
        {
            List<int[]> rows = new List<int[]>();

            StreamReader csv_reader = new StreamReader(filename);
            csv_reader.ReadLine();

            while (!csv_reader.EndOfStream)
            {
                string row = csv_reader.ReadLine();
                string[] numbers = row.Split(',');
                int X = Convert.ToInt32(numbers[0]);
                int Y = Convert.ToInt32(numbers[1]);
                int Press = Convert.ToInt32(numbers[2]);

                int[] current_params = { X, Y, Press };
                Console.WriteLine(X + " " + Y + " " + Press);
                rows.Add(current_params);
            }

            return rows.ToArray();
        }
        public static void NormalizeSign(int[][] signdata)
        {
            int maxX = signdata[0][0];
            int minX = signdata[0][0];
            int maxY = signdata[0][1];
            int minY = signdata[0][1];

            for (int i = 1; i < signdata.Length; i++)
            {
                if (signdata[i][2] == 1)
                {
                    maxX = Math.Max(maxX, signdata[i][0]);
                    maxY = Math.Max(maxY, signdata[i][1]);
                    minX = Math.Min(minX, signdata[i][0]);
                    minY = Math.Min(minY, signdata[i][1]);
                }
            }

            for (int i = 0; i < signdata.Length; i++)
            {
                if (signdata[i][2] == 1)
                {
                    signdata[i][0] = signdata[i][0] - minX;
                    signdata[i][1] = signdata[i][1] - minY;
                }
            }
            maxX -= minX; maxY -= minY;

            double ScaleX = (double)req_size / (double)maxX;
            double ScaleY = (double)req_size / (double)maxY;

            for (int i = 0; i < signdata.Length; i++)
            {
                signdata[i][0] = (int)(signdata[i][0] * ScaleX);
                signdata[i][1] = (int)(signdata[i][1] * ScaleY);
            }
        }
        public static void DrawTo(int[][] sign, string filename)
        {
            Pen m_pen = new Pen(Color.Black);
            m_pen.Width = widht;
            Bitmap bsign = new Bitmap(req_size, req_size);
            Graphics sign_graph = Graphics.FromImage(bsign);

            sign_graph.Clear(Color.White);

            for (int i = 1; i < sign.Length; i++)
            {
                if ((sign[i - 1][2] == 1) && (sign[i][2] == 1))
                {
                    sign_graph.DrawLine(m_pen, sign[i - 1][0], sign[i - 1][1],
                                               sign[i][0], sign[i][1]);
                }
            }

            bsign.Save(filename);
        }
    }

    public static class Neural
    {
        private static bool initialized = false;
        private static NamedPipeServerStream namedPipeServer;
        private static Process p;

        public static void Init(string pathToPython, string pathToSkript)
        {
            namedPipeServer = new NamedPipeServerStream(
                "neural_pipe",
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte);

            p = new Process();
            ProcessStartInfo info = new ProcessStartInfo(pathToPython, pathToSkript);
            //info.UseShellExecute = false;
            //info.CreateNoWindow = true;
            p.StartInfo = info;
            p.Start();

            namedPipeServer.WaitForConnection();
            initialized = true;
        }

        public static int Auth(string filename)
        {
            if (!initialized)
                return -1;

            namedPipeServer.Write(Encoding.UTF8.GetBytes("Auth"), 0, 4);
            namedPipeServer.WaitForPipeDrain();
            namedPipeServer.Write(Encoding.UTF8.GetBytes(filename), 0, Encoding.UTF8.GetBytes(filename).Length);
            namedPipeServer.WaitForPipeDrain();

            byte[] answer = new byte[4];
            namedPipeServer.Read(answer, 0, 4);
            int result = BitConverter.ToInt32(answer, 0);
            return result;
        }

        public static void Close()
        {
            namedPipeServer.Write(Encoding.UTF8.GetBytes("Close"), 0, 5);
            p.WaitForExit();
            initialized = false;
        }
    }
}
