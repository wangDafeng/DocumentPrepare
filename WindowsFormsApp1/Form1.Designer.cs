namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.button1.Location = new System.Drawing.Point(156, 210);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 42);
            this.button1.TabIndex = 0;
            this.button1.Text = "保密检查";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.button2.Location = new System.Drawing.Point(156, 364);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(183, 42);
            this.button2.TabIndex = 1;
            this.button2.Text = "作业复查";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "文本文件|*.txt*|C#文件|*.cs|所有文件|*.*";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(59, 126);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "配置表";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(156, 126);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(280, 21);
            this.textBox1.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.ForeColor = System.Drawing.Color.Beige;
            this.button4.Location = new System.Drawing.Point(156, 288);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(183, 42);
            this.button4.TabIndex = 4;
            this.button4.Text = "影像检查";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(93, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "项目名";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(156, 36);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(183, 21);
            this.textBox2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(93, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 21);
            this.label2.TabIndex = 7;
            this.label2.Text = "工程名";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(156, 73);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(183, 21);
            this.textBox3.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(496, 450);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Picture-View-BKINFO";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
    }
}

