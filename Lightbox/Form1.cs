using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lightbox
{
    public partial class Form1 : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public Form1(Image img)
        {
            InitializeComponent();
            
            // max possible w/h
            int miW = SystemInformation.VirtualScreen.Width - 
                Lightbox.Properties.Settings.Default.Padding * 2;
            int miH = SystemInformation.VirtualScreen.Height - 
                Lightbox.Properties.Settings.Default.Padding * 2;
            
            // if we need to resize...
            if (img.Width > miW || img.Height > miH)
            {   
                double ratio = Math.Min((double)miW / img.Width, (double)miH / img.Height);
                this.Width = (int)Math.Floor(img.Width * ratio);
                this.Height = (int)Math.Floor(img.Height * ratio);
            }
            // if not, let's just use its size!
            else
            {
                this.Width = img.Width;
                this.Height = img.Height;
            }

            pictureBox1.Image = img;
            // push us to the foreground
            this.Focus();

            // set up events to close the window et al
            this.Click += onClick;
            this.LostFocus += onClick;
            this.KeyDown += onKey;
            this.Closed += onClosed;
        }

        // Close action: toggled by 
        // clicking and the window losing focus. 
        public void onClick(object sender, EventArgs args)
        {
            Application.Exit();
        }

        public void onKey(object sender, KeyEventArgs args)
        {

        }
        
        public void onClosed(object sender, EventArgs args)
        {
            pictureBox1.Image.Dispose();
        }

        //  emulate the behaviour of windows with borders for moving and resizing
        protected override void WndProc(ref Message m) {
            const int gripSize = 16;
            const int captionSize = 32;

            const int WM_NCHITTEST = 0x84;
            const int WM_SETCUROR = 0x20;

            IntPtr HTCAPTION = (IntPtr)2;
            IntPtr HTBOTTOMRIGHT = (IntPtr)17;

            if (m.Msg == WM_NCHITTEST) {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < captionSize) {
                    m.Result = HTCAPTION;
                    return;
                }
                if (pos.X >= this.ClientSize.Width - gripSize && pos.Y >= this.ClientSize.Height - gripSize) {
                    m.Result = HTBOTTOMRIGHT;
                    return;
                }
            }
            else if (m.Msg == WM_SETCUROR) {
                if ((m.LParam.ToInt32() & 0xffff) == (int)HTCAPTION) {
                    Cursor.Current = Cursors.Hand;
                    m.Result = (IntPtr)1;  // Processed
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
