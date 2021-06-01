using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SignClient
{
    struct Discret
    {
        public Point coordinate;
        public bool MousePressed;
    }
    class Sign
    {
        private List<Discret> points = new List<Discret>();
        private Pen pencil = new Pen(Color.Black, 5);
        private StreamReader File;
        public void AddPoint(Point point, bool MousePressed) 
        {
            points.Add(new Discret { coordinate = point, MousePressed = MousePressed });
        }

        public void DrawPoints(PaintEventArgs e) 
        {
            for (int i=0; i<points.Count-1; i++)
            {
                if(points[i].MousePressed)
                    e.Graphics.DrawLine(pencil, points[i].coordinate, points[i+1].coordinate);
            }
        }

        public void EndSign()
        {
            int i = points.Count-1;
            
            while (points[i].MousePressed!=true && i>=0)
            {
                points.RemoveAt(i);
                i--;
            }
        }

        public void SaveToFile(string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName, false);
            writer.WriteLine("X,Y,Press");

            foreach (Discret point in points)
            {
                int press = 0;
                if (point.MousePressed)
                    press = 1;
                writer.WriteLine(point.coordinate.X.ToString() + "," + point.coordinate.Y.ToString() + "," + press.ToString());
            }
            writer.Flush();
            writer.Close();
        }

        public string GetSignData()
        {
            string result = "";

            foreach (Discret point in points)
            {
                int press = 0;
                if (point.MousePressed)
                    press = 1;
                result += point.coordinate.X.ToString() + "," 
                       +  point.coordinate.Y.ToString() + ","
                       +  press.ToString() + " ";
            }

            return result;
        }

        public void ReadFromFile(string fileName)
        {
            points.Clear();
            StreamReader reader = new StreamReader(fileName);
            reader.ReadLine();
            while(!reader.EndOfStream)
            {
                string row = reader.ReadLine();
                string[] numbers = row.Split(new char[] { ',' });

                Point point = new Point(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]));
                bool MousePressed = numbers[2] == "1";
                points.Add(new Discret { coordinate = point, MousePressed = MousePressed });
            }
            reader.Close();
        }

        public void ClearSign()
        {
            points.Clear();
        }

        public void StartAnimation(string fileName)
        {
            points.Clear();
            File = new StreamReader(fileName);
            File.ReadLine();
            
        }

        public bool AddPointFile()
        {
            if(File == null) return false;
            if (File.EndOfStream == true)
            {
                File.Close();
                File = null;
                return false;
            }
            string row = File.ReadLine();
            string[] numbers = row.Split(new char[] { ',' });

            Point point = new Point(Convert.ToInt32(numbers[0]), Convert.ToInt32(numbers[1]));
            bool MousePressed = numbers[2] == "1";
            points.Add(new Discret { coordinate = point, MousePressed = MousePressed });

            return true;
        }
    }
}
