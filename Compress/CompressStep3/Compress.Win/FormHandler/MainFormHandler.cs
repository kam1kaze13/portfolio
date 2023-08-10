using Compress.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win.ViewModels
{
    public class MainFormHandler
    {
        private DataGridView dataGridView;
        private ToolStripTextBox tbAddress;
        private PackageNavigation navigation;

        public MainFormHandler(DataGridView dataGridView, ToolStripTextBox tbAddress)
        {
            this.dataGridView = dataGridView;
            this.tbAddress = tbAddress;
            navigation = new PackageNavigation();
        }
        
        public void Load()
        {
            if (tbAddress.Text.Contains(".pkg"))
                navigation.OpenPackage(this.dataGridView, this.tbAddress);
            else
                navigation.SearchFiles(this.dataGridView, this.tbAddress);
        }

        public void bAddClick()
        {
            if (tbAddress.Text != "")
            {
                var selectedFiles = dataGridView.SelectedRows;

                List<string> paths = new List<string>();
                if (selectedFiles.Count != 0)
                {
                    foreach (DataGridViewRow row in selectedFiles)
                    {
                        paths.Add(row.Cells[1].Value.ToString());
                    }
                }
                AddForm addForm = new AddForm(tbAddress.Text,paths);
                addForm.Show();
                var tmpDataGridView = dataGridView;
                var tmpTbAddress = tbAddress;
                addForm.FormClosed += (_, args) =>
                {
                    if (tmpTbAddress.Text.Contains(".pkg"))
                    {
                        navigation.OpenPackage(tmpDataGridView, tmpTbAddress);
                    }
                    else if (Directory.Exists(tmpTbAddress.Text))
                    {
                        navigation.SearchFiles(tmpDataGridView, tmpTbAddress);
                    }
                };
                dataGridView = tmpDataGridView;
                tbAddress = tmpTbAddress;
            }
        }

        public void bExtractClick()
        {
            if (tbAddress.Text.Contains(".pkg"))
            {
                var selectedFiles = dataGridView.SelectedRows;
                if (selectedFiles.Count != 0)
                {
                    List<string> paths = new List<string>();

                    foreach (DataGridViewRow row in selectedFiles)
                    {
                        paths.Add(row.Cells[1].Value.ToString());
                    }

                    var packagePath = tbAddress.Text.Substring(0, tbAddress.Text.IndexOf(".pkg") + 4);
                    ExtractForm extractForm = new ExtractForm(packagePath, paths);
                    extractForm.Show();
                }
                else
                {
                    MessageBox.Show("No files selected!");
                }
            }
        }

        public void bDeleteClick()
        {
            if (tbAddress.Text.Contains(".pkg"))
            {
                var selectedFiles = dataGridView.SelectedRows;
                if (selectedFiles.Count != 0)
                {
                    DialogResult result = MessageBox.Show("Do you really want to delete these files??", "Confirm action", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        var packagePath = tbAddress.Text.Substring(0, tbAddress.Text.IndexOf(".pkg") + 4);

                        List<string> paths = new List<string>();

                        foreach (DataGridViewRow row in selectedFiles)
                        {
                            paths.Add(row.Cells[1].Value.ToString());
                        }

                        PackageCommands.ExecuteDeleteCommand(packagePath, paths, updateProgressBar => { });

                        navigation.OpenPackage(dataGridView, tbAddress);

                        dataGridView.ClearSelection();
                    }
                }
                else
                {
                    MessageBox.Show("No files selected!");
                }
            }
        }

        public void bUpClick()
        {
            navigation.Up(dataGridView, tbAddress);           
        }

        public void dgCellMouseDoubleClick()
        {
            tbAddress.Text = Path.Combine(tbAddress.Text, dataGridView[1, dataGridView.CurrentRow.Index].Value.ToString());

            if (this.tbAddress.Text.Contains(".pkg"))
            {
                navigation.OpenPackage(dataGridView, tbAddress);
            }
            else if (File.Exists(tbAddress.Text))
            {
                try
                {
                    Process.Start(tbAddress.Text);
                }
                catch
                {
                    MessageBox.Show("Can't open this file!");
                }
                while (tbAddress.Text[tbAddress.Text.Length - 1] != '\\')
                {
                    tbAddress.Text = tbAddress.Text.Remove(tbAddress.Text.Length - 1, 1);
                }
                tbAddress.Text = PathHelper.RemoveEndSeparator(tbAddress.Text);
            }
            else if (Directory.Exists(tbAddress.Text))
            {
                navigation.SearchFiles(this.dataGridView, this.tbAddress);
            }
        }

        public void dgDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        public void dgDragDrop(DragEventArgs e)
        {
            if (this.tbAddress.Text.Contains(".pkg"))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (files != null && files.Any())
                {
                    var packagePath = tbAddress.Text.Substring(0, tbAddress.Text.IndexOf(".pkg") + 4);
                    var pathTo = packagePath.Substring(packagePath.IndexOf(".pkg") + 4);

                    PackageCommands.ExecuteAddCommand(packagePath, files.ToList(), updateProgressBar => { }, pathTo);

                    navigation.OpenPackage(dataGridView, tbAddress);
                }
            }
        }

        public void tbKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tbAddress.Text.Contains(".pkg"))
                    navigation.OpenPackage(this.dataGridView, this.tbAddress);
                else
                    navigation.SearchFiles(this.dataGridView, this.tbAddress);
            }
        }

        public void MouseMove(MouseEventArgs e)
        {
            var point = new Point(Cursor.Position.X, Cursor.Position.Y);
            if (!dataGridView.RectangleToScreen(dataGridView.ClientRectangle).Contains(point))
            {
                if (e.Button == MouseButtons.Left)
                {
                    System.Collections.Specialized.StringCollection filePath = new System.Collections.Specialized.StringCollection();
                    filePath.Clear();
                    if (!tbAddress.Text.Contains(".pkg"))
                    {
                        if (dataGridView.SelectedRows.Count > 0)
                        {
                            foreach (DataGridViewRow cell in dataGridView.SelectedRows)
                            {
                                filePath.Add(tbAddress.Text + "\\" + cell.Cells[1].Value.ToString().Replace("\"", ""));
                            }
                            DataObject dataObject = new DataObject();
                            dataObject.SetFileDropList(filePath);
                            dataGridView.DoDragDrop(dataObject, DragDropEffects.Copy);
                        }
                    }
                    else
                    {
                        var temp = Path.GetTempPath();
                        var packagePath = tbAddress.Text.Substring(0, tbAddress.Text.IndexOf(".pkg") + 4);

                        var paths = new List<string>();
                        foreach (DataGridViewRow cell in dataGridView.SelectedRows)
                        {
                            paths.Add(cell.Cells[1].Value.ToString().Replace("\"", ""));
                            filePath.Add(temp + "\\" + cell.Cells[1].Value.ToString().Replace("\"", ""));
                        }

                        PackageCommands.ExecuteExtractCommand(packagePath, temp, paths, updateProgressBar => { });

                        DataObject dataObject = new DataObject();

                        dataObject.SetFileDropList(filePath);
                        dataGridView.DoDragDrop(dataObject, DragDropEffects.Move);
                    }
                }
            }
        }
    }
}
