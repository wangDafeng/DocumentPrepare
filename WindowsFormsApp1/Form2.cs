using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        int fastIndex=0;
        // 保存打开图片的路径
        string imgPath = null;
        Image newbitmap = null;
        // 打开图片的目录
        string directory = null;

        // 目录下的图片集合
        List<string> imgArray = null;
        Point mouseDownPoint = new Point();

        string text1 = "Q:模糊化   W:保密   E:错位   R:黑影   T:扭曲"  ;
        string text2 = "Q:左   W:左前   E:右前   R:右";
        string text3 = "Q:其它  W：人  E:立杆或路灯   R:建筑   T:树";


        bool isMove = false;
        PropertyInfo pInfo;
        Rectangle rect;
        struct Data
        {
           public  string id;
           public  string info;
          
        }
        private Data IData = new Data();

        List<Data> ReviewDatas = new List<Data>();
        DataSet ReviewDataSet = new DataSet();//定义一个数据集
        enum Progress
        {
            Step1,Step2,Step3, //自查
            TypeReview ,      //复查
            Non,
            Done
            
        }
        private Progress progress = Progress.Non;


        public Form2(string FileName)
        {
            InitializeComponent();

            DoubleBuffered = true;

            pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |BindingFlags.NonPublic);
            rect = (Rectangle)pInfo.GetValue(pictureBox1, null);

            OpenPicture(FileName);
        }



       

        //右击跳到下一帧
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = GetIndex(imgPath);
                // 释放上一张图片的资源，避免保存的时候出现ExternalException异常
                newbitmap.Dispose();
                if (index != imgArray.Count - 1)
                {
                    fastIndex += 1;
                    SwitchImg(index + 1);
                }
                else
                {
                    fastIndex = 0;
                    SwitchImg(0);
                }
            }else if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;   //记录鼠标左键按下时位置  
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
                pictureBox1.Focus();    //鼠标滚轮事件(缩放时)需要picturebox有焦点  
            }


        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();    //鼠标在picturebox上时才有焦点，此时可以缩放  
            if (isMove)
            {
                int x, y;           //新的pictureBox1.Location(x,y)  
                int moveX, moveY;   //X方向，Y方向移动大小。  
                moveX = Cursor.Position.X - mouseDownPoint.X;
                moveY = Cursor.Position.Y - mouseDownPoint.Y;
                x = pictureBox1.Location.X + moveX;
                y = pictureBox1.Location.Y + moveY;
                if (x > 0) x = 0;
                if (y > 0) y = 0;
                if (x + pictureBox1.Width < panel1.Width) x = panel1.Width - pictureBox1.Width;
                if (y + pictureBox1.Height < panel1.Height) y = panel1.Height - pictureBox1.Height;

                pictureBox1.Location = new Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }


        private void pictureBox1_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;
            float zoomStep;
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;
            if (pictureBox1.Width > panel1.Width * 7)
            {
                zoomStep = (float)panel1.Width / pictureBox1.Width;
                pictureBox1.Width = Convert.ToInt32(zoomStep * pictureBox1.Width);
                pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                pictureBox1.Location = new Point(0,0);
            }
            else if (pictureBox1.Width > panel1.Width * 3)
            {
                zoomStep = (float)panel1.Width * 8 / pictureBox1.Width;
                pictureBox1.Width = Convert.ToInt32(zoomStep * pictureBox1.Width);
                pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
                VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
                pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);
            }
            else
            {
                zoomStep = (float)panel1.Width*4 / pictureBox1.Width;
                pictureBox1.Width = Convert.ToInt32(zoomStep * pictureBox1.Width);
                pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
                VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
                pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);
            }
            
        }


        private void modifyPicbox()
        {
            if (pictureBox1.Location.X != 0) return;

            float zoomStep;
            int x = MousePosition.X;
            int y = MousePosition.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;
            if (pictureBox1.Width > panel1.Width * 7)
            {
                //zoomStep = (float)panel1.Width / pictureBox1.Width;
                //pictureBox1.Width = Convert.ToInt32(zoomStep * pictureBox1.Width);
                //pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                //pictureBox1.Location = new Point(0, 0);
            }
            else if (pictureBox1.Width > panel1.Width * 3)
            {
                //zoomStep = (float)panel1.Width / pictureBox1.Width;
                //pictureBox1.Width = Convert.ToInt32(zoomStep * pictureBox1.Width);
                //pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                //pictureBox1.Location = new Point(0, 0);
            }
            else
            {
                zoomStep = (float)panel1.Width * 4 / pictureBox1.Width;
                pictureBox1.Width = Convert.ToInt32(zoomStep * pictureBox1.Width);
                pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
                VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
                pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);
            }
        }

        //实现锚点缩放(以鼠标所指位置为中心缩放)；  
        //步骤：  
        //①先改picturebox长宽，长宽改变量一样；  
        //②获取缩放后picturebox中实际显示图像的长宽，这里长宽是不一样的；  
        //③将picturebox的长宽设置为显示图像的长宽；  
        //④补偿picturebox因缩放产生的位移，实现锚点缩放。  
        //  注释：为啥要②③步？由于zoom模式的机制，把picturebox背景设为黑就知道为啥了。  
        //这里需要获取zoom模式下picturebox所显示图像的大小信息，添加 using System.Reflection；  
        //pictureBox1_MouseWheel事件没找到。。。手动添加，别忘在Form1.Designer.cs的“Windows 窗体设计器生成的代码”里加入：          
        //this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel)。  
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            int VX, VY;     //因缩放产生的位移矢量  
            float zoomStep = 1.2f;

            if (e.Delta > 0)    //放大  
            {
                if (pictureBox1.Width > panel1.Width *8) return;
                //第①步  
                pictureBox1.Width= Convert.ToInt32(zoomStep* pictureBox1.Width);
                pictureBox1.Height = Convert.ToInt32(zoomStep * pictureBox1.Height);
                //第②步  

            }
            zoomStep = 0.8f;
            if (e.Delta < 0)    //缩小  
            {
                //防止一直缩成负值  
                if (pictureBox1.Width < panel1.Width)
                    return;
                pictureBox1.Width = Math.Max(Convert.ToInt32(zoomStep * pictureBox1.Width),panel1.Width);
                pictureBox1.Height = Math.Max(Convert.ToInt32(zoomStep * pictureBox1.Height),panel1.Height);

            }
            //第④步，求因缩放产生的位移，进行补偿，实现锚点缩放的效果  
            VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
            VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
            pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);
        }


        // 打开图片
        public void OpenPicture(string FileName)
        {

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            imgPath = FileName;
            // 初始化图片集合
            directory = Path.GetDirectoryName(imgPath);
            imgArray = ImageManager.GetImgCollection(directory);
            newbitmap = (Bitmap)Image.FromFile(imgPath);
            pictureBox1.Image = newbitmap;
            Text = imgPath;
            resetDes(FileName);


        }
        // 上一张图片
        private void button2_Click(object sender, EventArgs e)
        {
            int index = GetIndex(imgPath);
            // 释放上一张图片的资源，避免保存的时候出现ExternalException异常
            newbitmap.Dispose();
            if (index == 0)
            {
                fastIndex = imgArray.Count - 1;
                SwitchImg(imgArray.Count - 1);
            }
            else
            {
                fastIndex -= 1;
                SwitchImg(index - 1);
            }
        }

        // 下一张图片
        private void button3_Click(object sender, EventArgs e)
        {
            int index = GetIndex(imgPath);
            // 释放上一张图片的资源，避免保存的时候出现ExternalException异常
            newbitmap.Dispose();
            if (index != imgArray.Count - 1)
            {
                fastIndex += 1;
                SwitchImg(index + 1);
            }
            else
            {
                fastIndex = 0;
                SwitchImg(0);
            }
        }

        // 获得打开图片在图片集合中的索引
        private int GetIndex(string imagepath)
        {
            if (fastIndex>=0 && imgArray[fastIndex].Equals(imagepath)) return fastIndex;

            int index = 0;
            for (int i = 0; i < imgArray.Count; i++)
            {
                if (imgArray[i].Equals(imagepath))
                {
                    index = i;
                    break;
                }
            }
            fastIndex = index;
            return index;
        }

        private void resetPictureBox()
        {
            pictureBox1.Width = Convert.ToInt32(panel1.Width);
            pictureBox1.Height = Convert.ToInt32(panel1.Height);
            pictureBox1.Location = new Point(0, 0);
        }


        int currentImageWidth;
        int currentImageHeight;
        // 切换图片的方法
        private void SwitchImg(int index)
        {
            newbitmap = Image.FromFile(imgArray[index]);
            resetPictureBox();
            pictureBox1.Image = newbitmap;
            currentImageWidth = newbitmap.Width;
            currentImageHeight = newbitmap.Height;
            imgPath = imgArray[index];
            Text = imgArray[index];

            resetDes(Text);

          

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Visible = !textBox1.Visible;
        }

        private void OnKey_Down(object sender , KeyEventArgs e)
        {

            if (Program.workType == Program.Mode.review)
            {
                progress = Progress.TypeReview;
            }
            else if(Program.workType==Program.Mode.view)
            {
                if (progress != Progress.Step2 && progress!=Progress.Step3)
                {
                    progress = Progress.Step1;
                }
            }else if (Program.workType == Program.Mode.quickCheck)
            {
                if (progress != Progress.Step2)
                {
                    progress = Progress.Step1;
                }
            }
            

            switch (e.KeyCode)
            {
                case Keys.D1: modifyPicbox(); break;
                case Keys.D2: resetPictureBox(); break;
                case Keys.Q: InsertData("1"); break;
                case Keys.W: InsertData("2"); break;
                case Keys.E: InsertData("3"); break;
                case Keys.R: InsertData("4"); break;
                case Keys.T: InsertData("5"); break;
                case Keys.G: break;
                case Keys.N: InsertData("n"); break;

            }
        }


        private void reviewProgress(string type,ref Data data)
        {
            if(progress== Progress.TypeReview)
            {
                
                switch (type)
                {
                    //case 1:
                    //    IData.info += "(通过)";
                    //    break;
                    case "n":
                        data.id = Path.GetFileName(this.Text);
                        data.info = "(不通过)";
                        progress = Progress.Done;
                        break;
                }
                
            }

        }
        private void viewProgress(string type, ref Data data)
        {


            if (progress == Progress.Step1)
            {
                //string[] ss = this.Text.Split(new char[] { '\\' });
                data.id = Path.GetFileName(Text);
                data.info = "";
                switch (type)
                {
                    case "1":

                        data.info += "模糊化^";
                        progress = Progress.Step2;
                        SetTextBox(text2);
                        break;

                    case "2":
                        data.info += "保密^";
                        progress = Progress.Step2;
                        SetTextBox(text2);
                        break;
                    case "3":
                        data.info += "错位^";
                        progress = Progress.Step2;
                        SetTextBox(text2);
                        break;

                    case "4":
                        data.info += "黑影^";
                        progress = Progress.Step2;
                        SetTextBox(text2);
                        break;
                    case "5":
                        data.info += "扭曲^";
                        progress = Progress.Step2;
                        SetTextBox(text2);
                        break;

                    default:
                        break;
                }



            }
            else if (progress == Progress.Step2)
            {

                switch (type)
                {
                    case "1":
                        data.info += "左^";
                        progress = Progress.Step3;
                        SetTextBox(text3);
                        break;

                    case "2":
                        data.info += "左前^";
                        progress = Progress.Step3;
                        SetTextBox(text3);
                        break;
                    case "3":
                        data.info += "右前^";
                        progress = Progress.Step3;
                        SetTextBox(text3);
                        break;

                    case "4":
                        data.info += "右^";
                        progress = Progress.Step3;
                        SetTextBox(text3);
                        break;
                    case "5":
                        break;


                    default:
                        break;
                }

            }
            else if (progress == Progress.Step3)
            {
                switch (type)
                {
                    case "1":
                        data.info += "其它";
                        progress = Progress.Done;
                        break;

                    case "2":
                        data.info += "人";
                        progress = Progress.Done;
                        break;
                    case "3":
                        data.info += "立杆或路灯";
                        progress = Progress.Done;
                        break;

                    case "4":
                        data.info += "建筑";
                        progress = Progress.Done;
                        break;
                    case "5":
                        data.info += "树";
                        progress = Progress.Done;
                        break;


                    default:
                        break;
                }
               
            }
        }
        private void checkProgress(string type, ref Data data)
        {

            if (progress == Progress.Step1)
            {
                data.id = Path.GetFileName(Text);
                data.info = "";
                switch (type)
                {
                    case "1":

                        data.info += "左^";
                        progress = Progress.Step2;
                        SetTextBox("Q:杆  W：建筑  E:其它");
                        break;

                    case "2":
                        data.info += "右^";
                        progress = Progress.Step2;
                        SetTextBox("Q:杆  W：建筑  E:其它");
                        break;
                    case "3":
                        data.info += "全屏自查";
                        progress = Progress.Done;
                        break;

                    default:
                        break;
                }



            }
            else if (progress == Progress.Step2)
            {

                switch (type)
                {
                    case "1":
                        data.info += "杆";
                        progress = Progress.Done;
                        break;

                    case "2":
                        data.info += "建筑";
                        progress = Progress.Done;
                        break;
                    case "3":
                        data.info += "其它";
                        progress = Progress.Done;
                        break;

                    default:
                        break;
                }

              

            }
           
        }

        private void InsertData(string type)
        {
            if (Program.workType == Program.Mode.view)
            {
                viewProgress(type, ref IData);
            }
            else if (Program.workType == Program.Mode.review)
            {
                reviewProgress(type, ref IData);
            }
            else if (Program.workType == Program.Mode.quickCheck)
            {
                checkProgress(type, ref IData);
            }

            if (progress == Progress.Done)
            {
                insertExcel();
                progress = Progress.Non;
                IData.id = "";
                IData.info = "";
            }


        }
      
        private void insertExcel()
        {


            if (IData.id.CompareTo("")==0)
            {
                SetTextBox( "没有id");
                return;
            }

            try
            {



                SqlConnection con = new SqlConnection();
                con.ConnectionString = "server=fwq;database=PicturePass;uid=sa;pwd=sa";
                con.Open();
                SqlCommand cmd =new SqlCommand();
                cmd.Connection = con;
                if (IData.info.IndexOf("不通过")>=0)
                {
                    cmd.CommandText="UPDATE [PicturePass].[dbo].[pass] set pass=0 where projectname='" + Program.ProjectName + "'And unitname='" + Program.UnitName + "'And id ='"+IData.id+"'";
               
 }
                else
                {
                    cmd.CommandText = "insert into pass(id, description,date,type,username,projectname,pass,unitname) values(@id, @des,@date,@type,@username,@projectname,@pass,@unitname)";
                    cmd.Parameters.AddWithValue("@id", IData.id);
                    cmd.Parameters.AddWithValue("@des", IData.info);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@type", Program.workType == Program.Mode.view ? "保密检查" : "影像检查");
                    string HostName = Dns.GetHostName(); //得到主机名
                    IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                    for (int i = 0; i < IpEntry.AddressList.Length; i++)
                    {
                        //从IP地址列表中筛选出IPv4类型的IP地址
                        //AddressFamily.InterNetwork表示此IP为IPv4,
                        //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                        if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                        {
                            HostName += "_" + IpEntry.AddressList[i].ToString();
                        }
                    }
                    cmd.Parameters.AddWithValue("@username", HostName);
                    cmd.Parameters.AddWithValue("@projectname", Program.ProjectName);
                    cmd.Parameters.AddWithValue("@pass", DBNull.Value);
                    cmd.Parameters.AddWithValue("@unitname", Program.UnitName);
                }

               
                cmd.ExecuteNonQuery();

                SetTextBox("数据库数据插入成功",Color.Green,0);

            }
            catch
            {
                SetTextBox("数据库数据插入失败", Color.Red, 0);

            }

 


            try
            {
                FileStream fs = new FileStream(Program.filePath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(IData.id + "#" + IData.info);
                sw.Close();
                fs.Close();
                int n = textBox1.TextLength;
                SetTextBox(textBox1.Text + "|数据插入成功", Color.Green, n);

            }
            catch
            {
                int n = textBox1.TextLength;
                SetTextBox(textBox1.Text + "|数据插入失败", Color.Red, n);

            }


        }
        private void readData()
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "server=fwq;database=PicturePass;uid=sa;pwd=sa";

                SqlCommand MyCommand = new SqlCommand("SELECT * FROM [PicturePass].[dbo].[pass] where projectname='" + Program.ProjectName + "'And unitname='" + Program.UnitName + "'", con); //定义一个数据库操作指令
                SqlDataAdapter SelectAdapter = new SqlDataAdapter();//定义一个数据适配器
                SelectAdapter.SelectCommand = MyCommand;//定义数据适配器的操作指令
                con.Open();//打开数据库连接
                SelectAdapter.SelectCommand.ExecuteNonQuery();//执行数据库查询指令
                con.Close();//关闭数据库
                SelectAdapter.Fill(ReviewDataSet);//填充数据集
                SetTextBox( "数据库数据读取成功");

            }
            catch
            {
                SetTextBox( "数据库数据读取失败");
            }

            try
            {
                FileStream fs = new FileStream(Program.filePath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line != null && !line.Equals(""))
                    {
                        string[] sd = line.Split('#');
                        if (sd.Length < 2) continue;
                        Data data = new Data();
                        data.id = sd[0];
                        data.info = sd[1];
                        ReviewDatas.Add(data);
                    }
                }
                sr.Close();
                fs.Close();


                SetTextBox(textBox1.Text + "|数据读取成功");

            }
            catch
            {
                SetTextBox(textBox1.Text + "|数据读取失败");
            }




        }

        private string LoadData(string id)
        {
            string info="";

           
           if(ReviewDataSet.Tables.Count>0)
           {
                for(int i = 0; i < ReviewDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ReviewDataSet.Tables[0].Rows[i];
                    if (id.Equals(dr["id"].ToString().Trim()) )
                    {
                        if (dr["pass"]!=DBNull.Value&& !Convert.ToBoolean(dr["pass"]))
                        {
                            info += "(不通过)";
                        }
                        info += dr["description"].ToString().Trim() + "--";
                        

                    }
                }
           }
            if (info.Length > 1) return info;

            foreach (Data data in ReviewDatas)
            {
                if (id.Equals(data.id))
                {
                    info += data.info+"--";
                }
            }
            return info;
        }


        private void resetDes(string value)
        {
            progress = Progress.Non;
            if (Program.workType == Program.Mode.view)
            {

               SetTextBox(text1);
            }
            else if (Program.workType == Program.Mode.review)
            {
                string id = Path.GetFileName(value);
                string info = "";
                info += LoadData(id);
                SetTextBox(info + "   不通：N");

            }
            else if (Program.workType == Program.Mode.quickCheck)
            {
                SetTextBox( "  Q:左   W:右   E:全屏自查");
            }

       

        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Visible = false;
            if (Program.workType == Program.Mode.review)
            {
                readData();
            }

        }

        private void SetTextBox(string text,Color color,int start)
        {
            textBox1.Text = text;
            textBox1.Select(start, textBox1.TextLength-1);
            textBox1.SelectionColor = color;
            textBox1.SelectionAlignment = HorizontalAlignment.Center;
            textBox1.Select(0, 0);
        }
        private void SetTextBox(string text)
        {
            textBox1.Text = text;
            textBox1.SelectAll();
            textBox1.SelectionColor = Color.Black;
            textBox1.SelectionAlignment = HorizontalAlignment.Center;
            textBox1.Select(0, 0);
        }

    }
}
