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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;

namespace SignClient
{
    public partial class MainForm : Form
    {
        public CertParams certParams = new CertParams();

        private TcpClient client;
        public SslStream sslStream;
        bool SignWasStarted = false;
        bool mousePressed = false;
        Point currentPosition = new Point(0, 0);
        Sign userSign = new Sign();
        int seed = -1;
        RSA rsaKey;

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

        private void ParseMessage(byte[] row, ref byte code, ref string message)
        {
            code = row[0];
            message = Encoding.Unicode.GetString(row, 2, row.Length - 3);
        }

        private void GenerateKey()
        {
            if (seed == -1)
                return;

            RSAParameters rsa_params = new RSAParameters();
            NumberGenerator gen = new NumberGenerator(seed);
            rsa_params.P = gen.P;
            rsa_params.Q = gen.Q;
            rsa_params.Modulus = gen.N;
            rsa_params.Exponent = new byte[] { 1, 0, 1 };
            rsa_params.InverseQ = gen.InverseQ;
            rsa_params.D = gen.D;
            rsa_params.DP = gen.DP;
            rsa_params.DQ = gen.DQ;
            rsaKey = RSA.Create(rsa_params);
        }

        private void GenerateSertificate()
        {
            if (rsaKey == null)
                return;

            if (!certParams.hasParams)
                return;

            string subject = "CN=" + certParams.subject;
            var certReq = new CertificateRequest(subject, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certReq.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
            certReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certReq.PublicKey, false));
            byte[] serialNumber = BitConverter.GetBytes(DateTime.Now.ToBinary());
            var certificate = certReq.CreateSelfSigned(
                DateTimeOffset.Now, 
                certParams.endDate);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");
            File.WriteAllText(certParams.savePath + "\\" + certParams.subject + ".crt",
                builder.ToString());

            if (certParams.exportSecret)
            {
                string sign_name = rsaKey.SignatureAlgorithm.ToUpper();
                builder = new StringBuilder();
                builder.AppendLine($"-----BEGIN {sign_name} PRIVATE KEY-----");
                builder.AppendLine(Convert.ToBase64String(rsaKey.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks));
                builder.AppendLine($"-----END {sign_name} PRIVATE KEY-----");
                File.WriteAllText(certParams.savePath + "\\" + certParams.subject + ".key", builder.ToString());
            }

            MessageBox.Show("Сертификат сохранен!", 
                "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GetParams()
        {
            CertSettings certsets = new CertSettings();
            certsets.ShowDialog(this);
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
                byte[] answer = GetMessage();
                byte code = 255;
                string message = "";
                ParseMessage(answer, ref code, ref message);
                if (code == 200)
                {
                    seed = Convert.ToInt32(message);
                    GenerateKey();
                    SaveCert.Enabled = true;
                    SignFile.Enabled = true;
                    CheckSignFile.Enabled = true;
                    EncryptFile.Enabled = true;
                    DecryptFile.Enabled = true;
                }
                userSign.ClearSign();
                pictureBox1.Invalidate();
                Tip.Text = "Выберите действие";
                btGetCertificate.Text = "Получить сертификат";
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void SaveCert_Click(object sender, EventArgs e)
        {
            GetParams();
            GenerateSertificate();
        }

        private void SignFile_Click(object sender, EventArgs e)
        {
            if (rsaKey == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл для подписи";
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Выберите место сохранения подписи";
            saveFileDialog.Filter = "Signature files (*.sign)|*.sign";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog.FileName;
            BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
            FileInfo info = new FileInfo(filePath);
            byte[] data = reader.ReadBytes((int)info.Length);
            reader.Close();
            byte[] sign_data = rsaKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(saveFileDialog.FileName));
            writer.Write(sign_data);
            writer.Flush();
            writer.Close();
            MessageBox.Show("Подпись сохранена!",
                "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CheckSignFile_Click(object sender, EventArgs e)
        {
            if (rsaKey == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл подписи для проверки";
            openFileDialog.Filter = "Signature files (*.sign)|*.sign|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog.FileName;
            BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
            FileInfo info = new FileInfo(filePath);
            byte[] sign_data = reader.ReadBytes((int)info.Length);
            reader.Close();

            openFileDialog.Title = "Выберите подписанный файл подписи для проверки хэша";
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            filePath = openFileDialog.FileName;
            reader = new BinaryReader(File.OpenRead(filePath));
            info = new FileInfo(filePath);
            byte[] data = reader.ReadBytes((int)info.Length);
            reader.Close();

            bool result = rsaKey.VerifyData(data, sign_data,
                HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            if (result)
                MessageBox.Show("Подпись действительна!",
                "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Подпись не соответствует представленным данным!",
                "Провал!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void EncryptFile_Click(object sender, EventArgs e)
        {
            if (rsaKey == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл для зашифровывания";
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Выберите место сохранения зашифрованного файла";
            saveFileDialog.Filter = "Encrypted files (*.rsa)|*.rsa";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog.FileName;
            BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
            FileInfo info = new FileInfo(filePath);
            byte[] data = reader.ReadBytes((int)info.Length);
            reader.Close();

            BinaryWriter writer = new BinaryWriter(File.OpenWrite(saveFileDialog.FileName));
            RSAParameters rsaparams = rsaKey.ExportParameters(true);
            int offset = 0;
            int data_size = data.Length;
            int block_size = 128;// rsaparams.Modulus.Length - 1;

            while (offset < data_size)
            {
                byte[] data256;
                if (data_size - offset >= block_size)
                {
                    data256 = new byte[block_size];
                    Array.Copy(data, offset, data256, 0, block_size);
                }
                else
                {
                    data256 = new byte[data_size - offset];
                    Array.Copy(data, offset, data256, 0, data_size - offset);
                }
                byte[] encr_data = rsaKey.Encrypt(data256, RSAEncryptionPadding.Pkcs1);
                byte[] resersed = new byte[256];
                Array.Fill<byte>(resersed, 0);
                Array.Copy(encr_data, 0, resersed, 0, encr_data.Length);
                writer.Write(resersed);
                writer.Flush();
                
                offset += block_size;
            }
            writer.Close();

            MessageBox.Show("Файл зашифрован!",
                "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DecryptFile_Click(object sender, EventArgs e)
        {
            if (rsaKey == null)
                return;

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Выберите файл для расшифровывания";
                openFileDialog.Filter = "Encrypted files (*.rsa)|*.rsa|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Выберите место сохранения расшифрованного файла";
                saveFileDialog.Filter = "All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = openFileDialog.FileName;
                BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
                FileInfo info = new FileInfo(filePath);
                byte[] encr_data = reader.ReadBytes((int)info.Length);
                reader.Close();

                RSAParameters rsaparams = rsaKey.ExportParameters(true);
                int offset = 0;
                int data_size = encr_data.Length;
                int block_size = 256;

                BinaryWriter writer = new BinaryWriter(File.OpenWrite(saveFileDialog.FileName));
                while (offset < data_size)
                {
                    byte[] data256;
                    if (data_size - offset >= block_size)
                    {
                        data256 = new byte[block_size];
                        Array.Copy(encr_data, offset, data256, 0, block_size);
                    }
                    else
                    {
                        data256 = new byte[data_size - offset];
                        Array.Copy(encr_data, offset, data256, 0, data_size - offset);
                    }
                    byte[] data = rsaKey.Decrypt(data256, RSAEncryptionPadding.Pkcs1);
                    writer.Write(data);
                    writer.Flush();

                    offset += block_size;
                }
                writer.Close();

                MessageBox.Show("Файл расшифрован!",
                    "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception exp)
            {
                MessageBox.Show("Не удалось расшифровать файл: " + exp.Message,
                    "Провал!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
