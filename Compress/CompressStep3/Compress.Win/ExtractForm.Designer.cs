
namespace Compress.Win
{
    partial class ExtractForm
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
            this.tbPath = new System.Windows.Forms.TextBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.lPath = new System.Windows.Forms.Label();
            this.bExtract = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOverwrite = new System.Windows.Forms.RadioButton();
            this.rbAsk = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(23, 42);
            this.tbPath.Name = "tbPath";
            this.tbPath.ReadOnly = true;
            this.tbPath.Size = new System.Drawing.Size(409, 20);
            this.tbPath.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(238, 83);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(274, 302);
            this.treeView1.TabIndex = 1;
            // 
            // lPath
            // 
            this.lPath.AutoSize = true;
            this.lPath.Location = new System.Drawing.Point(21, 19);
            this.lPath.Name = "lPath";
            this.lPath.Size = new System.Drawing.Size(264, 13);
            this.lPath.TabIndex = 2;
            this.lPath.Text = "Path to extract (if it\'s doesn\'t exists, it should be create)";
            // 
            // bExtract
            // 
            this.bExtract.Location = new System.Drawing.Point(28, 358);
            this.bExtract.Name = "bExtract";
            this.bExtract.Size = new System.Drawing.Size(78, 26);
            this.bExtract.TabIndex = 3;
            this.bExtract.Text = "Ok";
            this.bExtract.UseVisualStyleBackColor = true;
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(131, 358);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(78, 26);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(29, 397);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(482, 18);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbOverwrite);
            this.groupBox1.Controls.Add(this.rbAsk);
            this.groupBox1.Location = new System.Drawing.Point(29, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 87);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Overwrite mode";
            // 
            // rbOverwrite
            // 
            this.rbOverwrite.AutoSize = true;
            this.rbOverwrite.Location = new System.Drawing.Point(11, 64);
            this.rbOverwrite.Name = "rbOverwrite";
            this.rbOverwrite.Size = new System.Drawing.Size(150, 17);
            this.rbOverwrite.TabIndex = 1;
            this.rbOverwrite.TabStop = true;
            this.rbOverwrite.Text = "Overwrite without question";
            this.rbOverwrite.UseVisualStyleBackColor = true;
            // 
            // rbAsk
            // 
            this.rbAsk.AutoSize = true;
            this.rbAsk.Checked = true;
            this.rbAsk.Location = new System.Drawing.Point(11, 26);
            this.rbAsk.Name = "rbAsk";
            this.rbAsk.Size = new System.Drawing.Size(101, 17);
            this.rbAsk.TabIndex = 0;
            this.rbAsk.TabStop = true;
            this.rbAsk.Text = "Ask to overwrite";
            this.rbAsk.UseVisualStyleBackColor = true;
            // 
            // ExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 421);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bExtract);
            this.Controls.Add(this.lPath);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.tbPath);
            this.Name = "ExtractForm";
            this.Text = "ExtractForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label lPath;
        private System.Windows.Forms.Button bExtract;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOverwrite;
        private System.Windows.Forms.RadioButton rbAsk;
    }
}