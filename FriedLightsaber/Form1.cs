using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FriedLightsaber
{
    public partial class Form1 : Form
    {
        private const bool SingleMode = false;
        private bool mouseDown = false;
        IntPtr currentWindow = IntPtr.Zero;
        public Form1()
        {
            InitializeComponent();
        }

        public void MouseClicked(object sender, MouseButtons mouseButtons, bool down)
        {
            if (mouseButtons == MouseButtons.Left)
            {
                //if (((mouseDown == false) && down) && SingleMode)
                //{
                //Point cursorPosition;
                //GetCursorPos(out cursorPosition);
                //currentWindow = WindowFromPoint(cursorPosition);
                //Text = DateTime.Now.ToString();
                //if (!Compiled.Contains(currentWindow))
                //{
                //    TransparencyHelper.SetWindowTransparency(currentWindow);
                //    Compiled.Add(currentWindow);
                //}
                //}
                mouseDown = down;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point p);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);


        List<IntPtr> Compiled = new List<IntPtr>();

        public void MouseMoved(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                // Get the screen coordinates of the cursor
                Point cursorPosition;
                GetCursorPos(out cursorPosition);

                // Get the handle of the window at the cursor position
                IntPtr windowHandle = currentWindow;
                if (!SingleMode)
                    windowHandle = WindowFromPoint(cursorPosition);

                // Convert the screen coordinates to client coordinates of the window
                ScreenToClient(windowHandle, ref cursorPosition);

                // Draw on the window here
                using (Graphics g = Graphics.FromHwnd(windowHandle))
                {
                    // Calculate the bounding rectangle for the circle centered at the cursor position
                    int radius = 25; // Adjust the size of the circle as needed
                    Rectangle circleBounds = new Rectangle(cursorPosition.X - radius, cursorPosition.Y - radius, 2 * radius, 2 * radius);

                    // Example: Draw a solid fuchsia (0xFF00FF) filled circle at the cursor position
                    using (SolidBrush brush = new SolidBrush(Color.Fuchsia))
                    {
                        g.FillEllipse(brush, circleBounds);
                    }
                }
            }
        }

        UserActivityHook actHook;
        private void Form1_Load(object sender, EventArgs e)
        {
            actHook = new UserActivityHook(); // create an instance with global hooks
                                              // hang on events
            actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
            actHook.OnMouseClickActivity += new UserActivityHook.MouseClickEventHandler(MouseClicked);
        }
    }
    public class TransparencyHelper
    {
        // Constants
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;

        // External functions
        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint transparentColor, short alpha, uint action); // true is OK, 0=transparent, 255=opaque

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        public static void SetWindowTransparency(IntPtr hWnd)
        {
            uint origionalStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, origionalStyle | WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 0xff00ff, 128, 0x1);
            //SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
    }
}
