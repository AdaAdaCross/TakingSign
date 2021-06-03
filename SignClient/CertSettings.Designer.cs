
namespace SignClient
{
    partial class CertSettings
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
            this.CertPath = new System.Windows.Forms.Label();
            this.SelectPath = new System.Windows.Forms.Button();
            this.SubjectName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.certEndDate = new System.Windows.Forms.DateTimePicker();
            this.ExportSecret = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Done = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CertPath
            // 
            this.CertPath.AutoSize = true;
            this.CertPath.Location = new System.Drawing.Point(54, 13);
            this.CertPath.Name = "CertPath";
            this.CertPath.Size = new System.Drawing.Size(248, 15);
            this.CertPath.TabIndex = 0;
            this.CertPath.Text = "<= Выберите путь сохранения сертификата";
            // 
            // SelectPath
            // 
            this.SelectPath.Location = new System.Drawing.Point(13, 9);
            this.SelectPath.Name = "SelectPath";
            this.SelectPath.Size = new System.Drawing.Size(35, 23);
            this.SelectPath.TabIndex = 1;
            this.SelectPath.Text = "...";
            this.SelectPath.UseVisualStyleBackColor = true;
            this.SelectPath.Click += new System.EventHandler(this.SelectPath_Click);
            // 
            // SubjectName
            // 
            this.SubjectName.Location = new System.Drawing.Point(102, 36);
            this.SubjectName.Name = "SubjectName";
            this.SubjectName.Size = new System.Drawing.Size(331, 23);
            this.SubjectName.TabIndex = 2;
            this.SubjectName.Text = "Default";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Имя субъекта";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Окончание действия";
            // 
            // certEndDate
            // 
            this.certEndDate.Location = new System.Drawing.Point(140, 65);
            this.certEndDate.Name = "certEndDate";
            this.certEndDate.Size = new System.Drawing.Size(293, 23);
            this.certEndDate.TabIndex = 5;
            // 
            // ExportSecret
            // 
            this.ExportSecret.AutoSize = true;
            this.ExportSecret.Location = new System.Drawing.Point(13, 100);
            this.ExportSecret.Name = "ExportSecret";
            this.ExportSecret.Size = new System.Drawing.Size(205, 19);
            this.ExportSecret.TabIndex = 6;
            this.ExportSecret.Text = "Экспортировать закрытый ключ";
            this.ExportSecret.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(367, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Внимание! Рекомендуется никогда не сохранять закрытый ключ.";
            // 
            // Done
            // 
            this.Done.Location = new System.Drawing.Point(277, 159);
            this.Done.Name = "Done";
            this.Done.Size = new System.Drawing.Size(75, 23);
            this.Done.TabIndex = 8;
            this.Done.Text = "Ок";
            this.Done.UseVisualStyleBackColor = true;
            this.Done.Click += new System.EventHandler(this.Done_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(358, 159);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 9;
            this.Cancel.Text = "Отмена";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // CertSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 189);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Done);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ExportSecret);
            this.Controls.Add(this.certEndDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SubjectName);
            this.Controls.Add(this.SelectPath);
            this.Controls.Add(this.CertPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CertSettings";
            this.Text = "Параметры сертификата";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CertPath;
        private System.Windows.Forms.Button SelectPath;
        private System.Windows.Forms.TextBox SubjectName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker certEndDate;
        private System.Windows.Forms.CheckBox ExportSecret;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Done;
        private System.Windows.Forms.Button Cancel;
    }
}