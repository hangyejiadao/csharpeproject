using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace 简易安防报警
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private WebCamera camera = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false; 
            button2.Enabled = false; 
            timer1.Enabled = false;
        }

        private void userControl12_Click(object sender, EventArgs e)
        {
            if (userControl12.Checked)
            { //初始化serialPort1控件，设置端口COM1,波特率为9600。
                serialPort1.PortName = "COM1";
                serialPort1.BaudRate = 9600;
                serialPort1.Open(); //打开端口 
                button1.Enabled = true;
                button2.Enabled = true;
                timer1.Enabled = true;
            }
            else
            {
                serialPort1.Close(); //关闭端口
                timer1.Enabled = false;
                button2.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //构建一个字节数组，用于存放打开继电器一路的指令。
            byte[] data = new byte[8]; 
            data[0] = 0x01;
            data[1] = 0x05;
            data[2] = 0x00;
            data[3] = 0x10;
            data[4] = 0xFF;
            data[5] = 0x00;
            data[6] = 0x8D;
            data[7] = 0xFF; //向COM1端口写入指令,继电器接收到指今后会按指令进行操作。
            serialPort1.Write(data,0,data.Count());
            button1.Enabled = false;
            button2.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[8];
            data[0] = 0x01; 
            data[1] = 0x05; 
            data[2] = 0x00; 
            data[3] = 0x10;
            data[4] = 0x00;
            data[5] = 0x00; 
            data[6] = 0xCC; 
            data[7] = 0x0F;
            serialPort1.Write(data, 0, data.Count());
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void userControl11_Load(object sender, EventArgs e)
        {
         
        }

        private void userControlClick(object sender, EventArgs e)
        {
            if (userControl11.Checked)
            {
                camera= new WebCamera(SPView.Handle, SPView.Width, SPView.Height);
                camera.StartWebCam();
            }
            else
            {
                camera.CloseWebcam();
            }
        }
    }
}
