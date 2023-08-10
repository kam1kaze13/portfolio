using Compress.Package;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using Compress.Win.ViewModels;

namespace Compress.Win
{
    public partial class AddForm : Form
    {     
        public AddForm(string path, List<string> paths = null)
        {
            InitializeComponent();

            this.handler = new AddFormHandler(textBox1, numericUpDown1, lOperation, lFileName, progressBar1);

            this.handler.Load(path,paths);

            #region Buttons click
            this.bAdd.Click += (_, e) => { this.handler.bAddClick(); };
            this.bBrowse.Click += (_, e) => { this.handler.bBrowseClick(); };
            this.bCancel.Click += (_, e) => { this.handler.bCancel(); };
            #endregion

            this.textBox1.TextChanged += (_, e) => { this.handler.tbChanged(); };           
        }

        private AddFormHandler handler;
    }
}
