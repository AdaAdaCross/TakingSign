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
        static void Main(string[] args)
        {
            string csv_path = "D:\\Soft_DSA\\csv";
            string png_path = "D:\\Soft_DSA\\png";

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
                    Worker.NormalizeSign(sign);
                    Worker.DrawTo(sign, png_file);
                }
            }
            //

            //

            //
            Console.ReadKey();
        }
    }
}
