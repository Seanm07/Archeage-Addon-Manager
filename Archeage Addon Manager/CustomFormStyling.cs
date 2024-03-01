using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Archeage_Addon_Manager {

    public class CompositedForm : Form {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            // I don't know why but unless we refresh twice after minimizing the window, the form doesn't redraw properly
            // Only happens due to the WS_EX_COMPOSITED custom form styling which fixes flickering when creating controls
            this.Refresh();
            this.Refresh();
        }
    }

    public class ToolstripRenderer : ToolStripProfessionalRenderer {

        // Override the system default menu item background rendering
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;

            Rectangle bounds = new Rectangle(Point.Empty, new Size(item.Width - 3, item.Height));
            bounds.X += 2;

            if (item.IsOnDropDown) {
                // This is an item inside the dropdown
                // If the current item is being hovered/selected via keyboard
                if (item.Selected) {
                    if (item.Enabled)
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 33, 35, 38)), bounds);
                } else {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(200, 33, 35, 38)), bounds);
                }
            } else {
                // This is the top level menu item container
                // If the current item is being hovered/selected via keyboard
                if (item.Selected || item.Pressed) {
                    if (item.Enabled)
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), bounds);
                }
            }


        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
            e.TextColor = Color.White;

            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;

            if (item.Image != null)
                g.DrawImage(item.Image, new Rectangle(13, 4, 13, 13));
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            //base.OnRenderToolStripBorder(e);
        }
    }

    public class CustomLabel : Label {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public CustomLabel() {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            UpdateStyles();
        }
    }

    public class CustomButton : Button {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public CustomButton() {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            UpdateStyles();
        }
    }

    public class CustomPanel : Panel {
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        private const int SB_VERT = 1;

        private int thumbMargin = 4;
        private int scrollPosition = 0; // Current scroll position
        private int scrollThumbSize = 50; // Size of the scrollbar thumb
        private bool thumbDragging = false;
        private int thumbDragOffset = 0;

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message m) {
            if ((m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL)
            && (((int)m.WParam & 0xFFFF) == 5)) {
                // Change SB_THUMBTRACK to SB_THUMBPOSITION
                m.WParam = (IntPtr)(((int)m.WParam & ~0xFFFF) | 4);
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public CustomPanel() {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            UpdateStyles();

            // Hide the system default vertical scrollbar
            ShowScrollBar(Handle, SB_VERT, 0);
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);

            // Hide the system default vertical scrollbar
            ShowScrollBar(Handle, SB_VERT, 0);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (AutoScroll) {
                int trackWidth = SystemInformation.VerticalScrollBarWidth;
                int trackHeight = ClientSize.Height;

                int thumbWidth = trackWidth - (thumbMargin * 2);

                var bgBrush = new SolidBrush(Color.FromArgb(255, 33, 35, 38));
                var thumbBrush = new SolidBrush(Color.White);

                e.Graphics.FillRectangle(bgBrush, ClientSize.Width - trackWidth, 0, trackWidth, trackHeight);

                int thumbHeight = Math.Max(trackHeight * trackHeight / ContentHeight(), scrollThumbSize) - (thumbMargin * 2);
                int thumbPos = scrollPosition * (trackHeight - thumbHeight) / (ContentHeight() - trackHeight);

                e.Graphics.FillRectangle(thumbBrush, ClientSize.Width - trackWidth + thumbMargin, thumbPos + thumbMargin, thumbWidth, thumbHeight);

                bgBrush.Dispose();
                thumbBrush.Dispose();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            int scrollAmount = SystemInformation.MouseWheelScrollLines * e.Delta;
            scrollPosition = Math.Max(0, Math.Min(scrollPosition - scrollAmount, ContentHeight() - ClientSize.Height));

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left) {
                // Calculate thumb position and size
                int trackHeight = ClientSize.Height;
                int thumbHeight = Math.Max((int)((float)ClientSize.Height / ContentHeight() * trackHeight), scrollThumbSize);
                int thumbPos = (int)((float)scrollPosition / (ContentHeight() - ClientSize.Height) * (trackHeight - thumbHeight));

                int thumbMargin = 2; // Margin for the thumb
                int trackWidth = SystemInformation.VerticalScrollBarWidth; // Width of the vertical scroll area


                // TODO: Rework and cleanup, doesn't work
                // Calculate thumb bounds including margin
                Rectangle thumbBounds = new Rectangle(ClientSize.Width - trackWidth + thumbMargin, thumbPos + thumbMargin,
                                                       trackWidth - 2 * thumbMargin, thumbHeight - 2 * thumbMargin);

                // Check if the mouse is within the thumb bounds
                if (thumbBounds.Contains(e.Location)) {
                    // Start dragging the thumb
                    thumbDragging = true;
                    thumbDragOffset = e.Y - thumbBounds.Top;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (thumbDragging) {
                // Calculate new scroll position based on thumb drag
                int trackHeight = ClientSize.Height;
                int thumbHeight = Math.Max((int)((float)ClientSize.Height / ContentHeight() * trackHeight), scrollThumbSize);

                // Calculate new scroll position based on mouse position relative to thumb drag offset
                int newScrollPosition = (int)(((float)(e.Y - thumbDragOffset) / (trackHeight - thumbHeight)) * (ContentHeight() - ClientSize.Height));

                // Ensure new scroll position stays within valid range
                scrollPosition = Math.Max(0, Math.Min(newScrollPosition, ContentHeight() - ClientSize.Height));

                // Scroll the panel
                PerformScroll();

                Invalidate();
            }
        }

        private void PerformScroll() {
            // Calculate scroll percentage
            float scrollPercentage = (float)scrollPosition / (ContentHeight() - ClientSize.Height);

            // Scroll the panel
            int maxScroll = ContentHeight() - ClientSize.Height;
            int newScrollValue = (int)(scrollPercentage * (VerticalScroll.Maximum - VerticalScroll.Minimum));
            VerticalScroll.Value = Math.Min(newScrollValue, maxScroll);
        }


        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left) {
                // Stop thumb dragging
                thumbDragging = false;
            }
        }

        private int ContentHeight() {
            int totalHeight = 0;
            foreach (Control control in Controls) {
                totalHeight += control.Height;
            }
            return totalHeight;
        }
    }
}
