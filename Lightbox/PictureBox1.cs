using System;
using System.Windows.Forms;

namespace Lightbox
{
    public partial class PictureBox1 : PictureBox
    {
        // forward all NCHITTEST to the parent
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = -1;
        
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
