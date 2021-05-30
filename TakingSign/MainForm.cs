using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace TakingSign
{
    public partial class MainForm : Form
    {
        bool SignWasStarted = false;
        bool mousePressed = false;
        Point currentPosition = new Point(0, 0);
        Sign userSign = new Sign();
        int signNumber = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void DrawGrid (PaintEventArgs e)
        {
            for (int y = 50; y<500; y+=50)
            {
                e.Graphics.DrawLine(Pens.LightBlue, 0, y, 1000, y);
            }
            for (int x = 50; x < 1000; x += 50)
            {
                e.Graphics.DrawLine(Pens.LightBlue, x, 0, x, 500);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawString(currentPosition.ToString(), new Font("Arial", 20), Brushes.Black, 10, 10);
            //e.Graphics.DrawString(mousePressed.ToString(), new Font("Arial", 20), Brushes.Black, 10, 40);
            //e.Graphics.DrawLine(Pens.Black, 10, 10, 100, 100);
            DrawGrid(e);
            userSign.DrawPoints(e);
        }

        private void Record_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите логин!", "Ошибка!");
                return;
            }

            if (!SignWasStarted)
            {
                timer1.Enabled = true;
                Record.Text = "Остановить";
            }
            else
            {
                timer1.Enabled = false;
                SignWasStarted = false;
                userSign.EndSign();

                string filename = "SignData//"+ textBox1.Text + "_" + signNumber.ToString() + ".csv";

                if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\SignData\\"))
                {
                    Directory.CreateDirectory("SignData");
                }
                userSign.SaveToFile(filename);
                signNumber++;
                MessageBox.Show("Подпись сохранена");
                userSign.ClearSign();
                pictureBox1.Invalidate();
                Record.Text = "Запись";
            }
            
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            currentPosition = MousePosition;
            currentPosition = pictureBox1.PointToClient(currentPosition);
            mousePressed = Control.MouseButtons == MouseButtons.Left;

            if (currentPosition.X < 0 || currentPosition.X > 1000 || currentPosition.Y < 0 || currentPosition.Y > 500) return;
            if (SignWasStarted) userSign.AddPoint(currentPosition, mousePressed);

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!SignWasStarted)
            {
                userSign = new Sign();
            }
            if (timer1.Enabled)
            {
                SignWasStarted = true;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            signNumber = 0;
        }

        private void showFromFile_Click(object sender, EventArgs e)
        {
            if(SignWasStarted)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\SignData\\";
            openFileDialog.Filter = "csv tables (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                userSign.ReadFromFile(openFileDialog.FileName);
                pictureBox1.Invalidate();
            }

        }

        private void showSignDinamik_Click(object sender, EventArgs e)
        {
            if (SignWasStarted)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\SignData\\";
            openFileDialog.Filter = "csv tables (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //userSign.ReadFromFile(openFileDialog.FileName);
                userSign.StartAnimation(openFileDialog.FileName);
                timer2.Enabled = true;
                Record.Enabled = false;
                showFromFile.Enabled = false;
                pictureBox1.Invalidate();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            if (!userSign.AddPointFile())
            {
                timer2.Stop();
                Record.Enabled = true;
                showFromFile.Enabled = true;
            }
            pictureBox1.Invalidate();
        }
    }
}
