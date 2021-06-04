
namespace SignClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.btGetCertificate = new System.Windows.Forms.Button();
            this.Tip = new System.Windows.Forms.Label();
            this.SaveCert = new System.Windows.Forms.Button();
            this.SignFile = new System.Windows.Forms.Button();
            this.CheckSignFile = new System.Windows.Forms.Button();
            this.DecryptFile = new System.Windows.Forms.Button();
            this.EncryptFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(14, 14);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1000, 500);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // timer1
            // 
            this.timer1.Interval = 5;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 5;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // btGetCertificate
            // 
            this.btGetCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btGetCertificate.Location = new System.Drawing.Point(14, 550);
            this.btGetCertificate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btGetCertificate.Name = "btGetCertificate";
            this.btGetCertificate.Size = new System.Drawing.Size(208, 46);
            this.btGetCertificate.TabIndex = 1;
            this.btGetCertificate.Text = "Получить сертификат";
            this.btGetCertificate.UseVisualStyleBackColor = true;
            this.btGetCertificate.Click += new System.EventHandler(this.btGetCertificate_Click);
            // 
            // Tip
            // 
            this.Tip.AutoSize = true;
            this.Tip.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Tip.Location = new System.Drawing.Point(14, 517);
            this.Tip.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Tip.Name = "Tip";
            this.Tip.Size = new System.Drawing.Size(179, 20);
            this.Tip.TabIndex = 2;
            this.Tip.Text = "Выберите действие";
            // 
            // SaveCert
            // 
            this.SaveCert.Enabled = false;
            this.SaveCert.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SaveCert.Location = new System.Drawing.Point(230, 550);
            this.SaveCert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SaveCert.Name = "SaveCert";
            this.SaveCert.Size = new System.Drawing.Size(216, 46);
            this.SaveCert.TabIndex = 3;
            this.SaveCert.Text = "Сохранить сертификат";
            this.SaveCert.UseVisualStyleBackColor = true;
            this.SaveCert.Click += new System.EventHandler(this.SaveCert_Click);
            // 
            // SignFile
            // 
            this.SignFile.Enabled = false;
            this.SignFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SignFile.Location = new System.Drawing.Point(568, 520);
            this.SignFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SignFile.Name = "SignFile";
            this.SignFile.Size = new System.Drawing.Size(216, 32);
            this.SignFile.TabIndex = 4;
            this.SignFile.Text = "Подписать файл";
            this.SignFile.UseVisualStyleBackColor = true;
            this.SignFile.Click += new System.EventHandler(this.SignFile_Click);
            // 
            // CheckSignFile
            // 
            this.CheckSignFile.Enabled = false;
            this.CheckSignFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CheckSignFile.Location = new System.Drawing.Point(568, 561);
            this.CheckSignFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CheckSignFile.Name = "CheckSignFile";
            this.CheckSignFile.Size = new System.Drawing.Size(216, 32);
            this.CheckSignFile.TabIndex = 5;
            this.CheckSignFile.Text = "Проверить подпись";
            this.CheckSignFile.UseVisualStyleBackColor = true;
            this.CheckSignFile.Click += new System.EventHandler(this.CheckSignFile_Click);
            // 
            // DecryptFile
            // 
            this.DecryptFile.Enabled = false;
            this.DecryptFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DecryptFile.Location = new System.Drawing.Point(792, 561);
            this.DecryptFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DecryptFile.Name = "DecryptFile";
            this.DecryptFile.Size = new System.Drawing.Size(216, 32);
            this.DecryptFile.TabIndex = 7;
            this.DecryptFile.Text = "Расшифровать файл";
            this.DecryptFile.UseVisualStyleBackColor = true;
            this.DecryptFile.Click += new System.EventHandler(this.DecryptFile_Click);
            // 
            // EncryptFile
            // 
            this.EncryptFile.Enabled = false;
            this.EncryptFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EncryptFile.Location = new System.Drawing.Point(792, 520);
            this.EncryptFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.EncryptFile.Name = "EncryptFile";
            this.EncryptFile.Size = new System.Drawing.Size(216, 32);
            this.EncryptFile.TabIndex = 6;
            this.EncryptFile.Text = "Зашифровать файл";
            this.EncryptFile.UseVisualStyleBackColor = true;
            this.EncryptFile.Click += new System.EventHandler(this.EncryptFile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 605);
            this.Controls.Add(this.DecryptFile);
            this.Controls.Add(this.EncryptFile);
            this.Controls.Add(this.CheckSignFile);
            this.Controls.Add(this.SignFile);
            this.Controls.Add(this.SaveCert);
            this.Controls.Add(this.Tip);
            this.Controls.Add(this.btGetCertificate);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1037, 644);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1037, 644);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button btGetCertificate;
        private System.Windows.Forms.Label Tip;
        private System.Windows.Forms.Button SaveCert;
        private System.Windows.Forms.Button SignFile;
        private System.Windows.Forms.Button CheckSignFile;
        private System.Windows.Forms.Button DecryptFile;
        private System.Windows.Forms.Button EncryptFile;
    }
}