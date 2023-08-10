using Compress.Package;
using Compress.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win
{
    public partial class ExtractForm : Form
    {
        public ExtractForm(string packagePath, List<string> paths)
        {
            InitializeComponent();

            this.handler = new ExtractFormHandler(tbPath, treeView1, rbAsk, progressBar1);

            this.handler.Load(packagePath, paths);

            #region Buttons Click
            this.bExtract.Click += (_, e) => { this.handler.bExtractClick(); };
            this.bCancel.Click += (_, e) => { this.handler.bCancelClick(); };
            #endregion

            #region TreeView Handlers
            this.treeView1.BeforeExpand += (_, e) => { this.handler.BeforeExpand(e); };
            this.treeView1.BeforeSelect += (_, e) => { this.handler.BeforeSelect(e); };
            this.treeView1.NodeMouseDoubleClick += (_, e) => { this.handler.NodeMouseDoubleClick(e); };
            #endregion
        }

        private ExtractFormHandler handler;        
    }
}
