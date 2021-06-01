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
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;


namespace SignClient
{
    public partial class MainForm : Form
    {
        private TcpClient client;
        public SslStream sslStream;
        bool SignWasStarted = false;
        bool mousePressed = false;
        Point currentPosition = new Point(0, 0);
        Sign userSign = new Sign();
        int signNumber = 0;

        public bool CheckCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            string startDate_str = certificate.GetEffectiveDateString();
            string endDate_str = certificate.GetExpirationDateString();
            DateTime now = DateTime.Now;
            DateTime startDate = Convert.ToDateTime(startDate_str);
            DateTime endDate = Convert.ToDateTime(endDate_str);

            if (now < startDate)
                return false;
            if (now > endDate)
                return false;

            return true;
        }


        public MainForm()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckCert;

                client = new TcpClient("192.168.56.1", 56565);
                sslStream = new SslStream(client.GetStream(), false, ServicePointManager.ServerCertificateValidationCallback);
                sslStream.AuthenticateAsClient("192.168.56.1");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Connection error!");
            }
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
            DrawGrid(e);
            userSign.DrawPoints(e);
        }

        private void Record_Click(object sender, EventArgs e)
        {
            
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

        private void timer2_Tick(object sender, EventArgs e)
        {

            if (!userSign.AddPointFile())
            {
                timer2.Stop();
            }
            pictureBox1.Invalidate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                sslStream.Close();
            }
            catch (Exception exp) {}
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
                MessageBox.Show("Error: " + exp.Message);
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
                MessageBox.Show("Error: " + exp.Message);
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

        private void btGetCertificate_Click(object sender, EventArgs e)
        {
            if (!SignWasStarted)
            {
                timer1.Enabled = true;
                Tip.Text = "Наносите подпись";
                btGetCertificate.Text = "Остановить";
            }
            else
            {
                timer1.Enabled = false;
                SignWasStarted = false;
                userSign.EndSign();

                string str_sign = userSign.GetSignData();
                byte[] mess = GenerateMessage(1, str_sign);
                SendMessage(mess);
                //userSign.SaveToFile(filename);
                //MessageBox.Show("Подпись сохранена");
                userSign.ClearSign();
                pictureBox1.Invalidate();
                Tip.Text = "Выберите действие";
                btGetCertificate.Text = "Получить сертификат";
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
