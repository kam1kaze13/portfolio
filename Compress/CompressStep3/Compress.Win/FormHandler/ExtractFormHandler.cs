using Compress.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win.ViewModels
{
    public class ExtractFormHandler
    {
        public ExtractFormHandler(TextBox tbPath, TreeView treeView, RadioButton rbAsk, ProgressBar progressBar)
        {
            this.tbPath = tbPath;
            this.treeView = treeView;
            this.rbAsk = rbAsk;
            this.progressBar = progressBar;
        }

        public void Load(string packagePath, List<string> paths)
        {
            this.packagePath = packagePath;
            this.pathTo = this.packagePath.Remove(this.packagePath.IndexOf(".pkg"));
            this.paths = paths;
            this.tbPath.Text = this.pathTo;
            FillDriveNodes();
        }

        public void BeforeExpand(TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public void BeforeSelect(TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void FillDriveNodes()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode { Text = drive.Name };
                    FillTreeNode(driveNode, drive.Name);
                    this.treeView.Nodes.Add(driveNode);
                }
            }
            catch (Exception ex) { }
        }

        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
                    driveNode.Nodes.Add(dirNode);
                }
            }
            catch (Exception ex) { }
        }

        public void NodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            this.tbPath.Text = e.Node.FullPath;
            this.pathTo = this.tbPath.Text;
        }

        public void bExtractClick()
        {
            if (this.paths.Count != 0)
            {
                var answer = rbAsk.Checked;

                PackageCommands.ExecuteExtractCommand(this.packagePath, this.pathTo, this.paths, updateProgressBar => { this.UpgradeProgressBar(updateProgressBar); }, answer);

                this.paths = null;

                ExtractForm.ActiveForm.Close();
            }
            else
            {
                MessageBox.Show("No files selected!");
                ExtractForm.ActiveForm.Close();
            }
        }

        public void bCancelClick()
        {
            ExtractForm.ActiveForm.Close();
        }

        private void UpgradeProgressBar(FileProcessingEventArgs e)
        {
            this.progressBar.Visible = true;

            this.progressBar.Maximum = (int)e.Total;
            this.progressBar.Value = (int)e.TotalProcessed;

            if (this.progressBar.Value == this.progressBar.Maximum)
            {
                this.progressBar.Visible = false;
            }
        }


        private TextBox tbPath;
        private TreeView treeView;
        private RadioButton rbAsk;
        private ProgressBar progressBar;
        private string packagePath;
        private string pathTo;
        private List<string> paths;
    }
}
