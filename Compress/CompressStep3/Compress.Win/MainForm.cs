using Compress.Core;
using Compress.Package;
using Compress.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win
{
    public partial class MainForm : Form
    {       
        public MainForm(string path = "")
        {
            InitializeComponent();

            tbAddress.Text = path;

            this.handler = new MainFormHandler(dataGridView1, tbAddress);

            this.handler.Load();

            #region Buttons click
            this.bAdd.Click += (_, e) => { this.handler.bAddClick(); };
            this.bUp.Click += (_, e) => { this.handler.bUpClick(); };
            this.bExtract.Click += (_, e) => { this.handler.bExtractClick(); };
            this.bDelete.Click += (_, e) => { this.handler.bDeleteClick(); };
            #endregion

            #region DataGridView events
            this.dataGridView1.CellMouseDoubleClick += (_, e) => { this.handler.dgCellMouseDoubleClick(); };
            this.dataGridView1.DragDrop += (_, e) => { this.handler.dgDragDrop(e); };
            this.dataGridView1.DragEnter += (_, e) => { this.handler.dgDragEnter(e); };
            this.dataGridView1.MouseMove += (_, e) => { this.handler.MouseMove(e); };
            #endregion

            #region TextBox Address
            this.tbAddress.KeyDown += (_, e) => { this.handler.tbKeyDown(e); };
            #endregion
        }

        private MainFormHandler handler;
    }
}
