using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignClient
{
    public partial class CertSettings : Form
    {
        public MainForm parent;
        private string savepath = "";

        public CertSettings()
        {
            InitializeComponent();
            certEndDate.Value = DateTime.Now.AddYears(1);
        }

        private void SelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFileDialog = new FolderBrowserDialog();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                savepath = saveFileDialog.SelectedPath;
                CertPath.Text = savepath;
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            if (savepath == "")
            {
                MessageBox.Show("Не выбран путь сохранения",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (parent != null)
            {
                parent.certParams = new CertParams(savepath, 
                    SubjectName.Text, 
                    certEndDate.Value, 
                    ExportSecret.Checked);
            }
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (parent != null)
            {
                parent.certParams = new CertParams();
            }
            this.Close();
        }

        public void ShowDialog(MainForm _parent)
        {
            parent = _parent;
            this.ShowDialog();
        }
    }
}
