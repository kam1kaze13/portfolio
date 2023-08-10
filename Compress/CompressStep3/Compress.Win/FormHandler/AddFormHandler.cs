using Compress.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win.ViewModels
{
    public class AddFormHandler
    {      
        public AddFormHandler(TextBox tbAddress, NumericUpDown numericUpDown, Label lOperation, Label lFileName, ProgressBar progressBar)
        {
            this.tbAddress = tbAddress;
            this.numericUpDown = numericUpDown;
            this.lOperation = lOperation;
            this.lFileName = lFileName;
            this.progressBar = progressBar;
        }
        public void Load(string path,List<string> paths = null)
        {
            if (paths != null)
                this.paths = paths;

            if (path.Contains(".pkg"))
            {
                this.packageTo = path.Substring(path.IndexOf(".pkg") + 4);
                if (this.packageTo != "")
                {
                    this.archivePath = path.Replace(this.packageTo, String.Empty);
                    this.packageTo = this.packageTo.Remove(0, 1);
                }

                else
                    this.archivePath = path;
            }
            else
            {
                this.archivePath = path.Insert(path.Length, "\\package.pkg");
                this.packageTo = "";
            }

            tbAddress.Text = this.archivePath;
        }

        public void bAddClick()
        {
            if (paths != null)
            {
                PackageCommands.ExecuteAddCommand(this.archivePath, this.paths, updateProgressBar => { this.UpgradeProgressBar(updateProgressBar); }, this.packageTo, Convert.ToInt32(numericUpDown.Value));

                this.paths = null;

                AddForm.ActiveForm.Close();
            }
            else
            {
                MessageBox.Show("No files selected!");
            }
        }

        public void bBrowseClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*"; ;
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                paths = openFileDialog.FileNames.ToList();
            }
        }

        public void bCancel()
        {
            AddForm.ActiveForm.Close();
        }

        public void tbChanged()
        {
            this.archivePath = tbAddress.Text;
        }

        private void UpgradeProgressBar(FileProcessingEventArgs e)
        {
            progressBar.Visible = true;
            lOperation.Visible = true;
            lFileName.Visible = true;

            progressBar.Maximum = (int)e.Total;
            progressBar.Value = (int)e.TotalProcessed;
            lOperation.Text = e.Type.ToString();
            lFileName.Text = e.FileName;

            if (progressBar.Value == progressBar.Maximum)
            {
                progressBar.Visible = false;
                lOperation.Visible = false;
                lFileName.Visible = false;
            }
        }

        private TextBox tbAddress;
        private NumericUpDown numericUpDown;
        private Label lOperation;
        private Label lFileName;
        private ProgressBar progressBar;
        private string archivePath;
        private string packageTo;
        private List<string> paths;

    }
}
