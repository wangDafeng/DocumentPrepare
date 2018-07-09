using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Form2 f;

        public Form1()
        {
            InitializeComponent();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "server=fwq;database=PicturePass;uid=sa;pwd=sa";

                SqlCommand MyCommand = new SqlCommand("SELECT * FROM [PicturePass].[dbo].[projectname]", con); //定义一个数据库操作指令
                SqlDataAdapter SelectAdapter = new SqlDataAdapter();//定义一个数据适配器
                SelectAdapter.SelectCommand = MyCommand;//定义数据适配器的操作指令
                DataSet MyDataSet = new DataSet();//定义一个数据集
                con.Open();//打开数据库连接
                SelectAdapter.SelectCommand.ExecuteNonQuery();//执行数据库查询指令
                con.Close();//关闭数据库

                SelectAdapter.Fill(MyDataSet);//填充数据集
                textBox2.Text= MyDataSet.Tables[0].Rows[0]["name"].ToString().Trim();
            }
            catch
            {

            }
        


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.workType = Program.Mode.view;
            showForm2();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.workType = Program.Mode.review;
            showForm2();


        }

        private void showForm2()
        {

            if (textBox2.Text.Length < 3)
            {
                MessageBox.Show("项目名过短");
                return;
            }
            if (textBox3.Text.Length < 3)
            {
                MessageBox.Show("工程名过短");
                return;
            }


            Program.ProjectName = textBox2.Text;
            Program.UnitName = textBox3.Text;
  


            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "图片文件(*.jpg;*.bmp;*.png)|*.jpg;*.bmp;*.png|(All file(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {

                    if (f == null || f.IsDisposed)
                    {
                        f = new Form2(fileDialog.FileName);
                    }
                    f.ShowDialog();

                }
            }

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            Program.filePath = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Program.workType = Program.Mode.quickCheck;
            showForm2();
        }
    }
}
