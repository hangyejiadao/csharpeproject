using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 视频
{
    public partial class UserControl1 : UserControl
    {
        bool isCheck = false;
      
        public bool Checked   
           {
               set { isCheck = value; this.Invalidate(); }
               get { return isCheck; }
           }
       protected override void OnPaint(PaintEventArgs e) 
       { 
           Bitmap bitMapOn = null; 
           Bitmap bitMapOff = null;
           bitMapOn = global::视频.Properties.Resources.open;
           bitMapOff = global::视频.Properties.Resources.close;
           Graphics g = e.Graphics;
           Rectangle rec = new Rectangle(0, 0, this.Size.Width, this.Size.Height); 
           if (isCheck) 
           {
               g.DrawImage(bitMapOn, rec);
           } 
           else {
                  g.DrawImage(bitMapOff, rec);
           }
       }
         public UserControl1() 
               { 
             this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
             this.SetStyle(ControlStyles.DoubleBuffer, true);
             this.SetStyle(ControlStyles.ResizeRedraw, true); 
             this.SetStyle(ControlStyles.Selectable, true);
             this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
             this.SetStyle(ControlStyles.UserPaint, true);
             this.BackColor = Color.Transparent; 
             this.Cursor = Cursors.Hand; 
             this.Size = new Size(87, 27);
             InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        private void UserControl1_Click(object sender, EventArgs e)
        {
            isCheck = !isCheck;
            this.Invalidate();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
