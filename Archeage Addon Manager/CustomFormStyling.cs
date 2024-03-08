using System;
using System.Diagnostics;
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
        private bool thumbDragging = false;
        private int thumbDragOffset = 0;

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        private int scrollBgX, scrollBgY, scrollBgWidth, scrollBgHeight;
        private int panelContentsHeight, thumbWidth, thumbHeight, thumbPos;

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
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            this.Layout += LayoutReady;
        }

        // Called once the layout is ready and heights are no longer 0
        private void LayoutReady(object sender, LayoutEventArgs e) {
            RefreshLayout();
        }

        private void RefreshLayout() {
            panelContentsHeight = ContentHeight();

            if (panelContentsHeight > 0) {
                scrollBgWidth = SystemInformation.VerticalScrollBarWidth;
                scrollBgHeight = Height;
                scrollBgX = Width - scrollBgWidth;
                scrollBgY = 0;

                thumbWidth = scrollBgWidth - (thumbMargin * 2);
                thumbHeight = (int)((float)scrollBgHeight * ((float)scrollBgHeight / (float)panelContentsHeight));

                PerformScroll();
            }
        }

        protected override void OnSizeChanged(EventArgs e) {
            // Hide the system default vertical scrollbar
            ShowScrollBar(Handle, SB_VERT, 0);

            RefreshLayout();
        }

        private void PerformScroll() {
            thumbHeight = (int)((float)scrollBgHeight * ((float)scrollBgHeight / (float)panelContentsHeight));
            thumbPos = (int)((float)scrollPosition * ((float)scrollBgHeight - (float)thumbHeight) / ((float)panelContentsHeight - (float)scrollBgHeight));

            // Calculate scroll percentage
            float scrollPercentage = (float)scrollPosition / ((float)panelContentsHeight - (float)Height);

            // Scroll the panel
            VerticalScroll.Value = (int)(scrollPercentage * (VerticalScroll.Maximum - Height - VerticalScroll.Minimum));

            //Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e) {
            //base.OnPaint(e);

            if (AutoScroll && panelContentsHeight > thumbHeight) {
                // Draw the scrollbar background
                using (var bgBrush = new SolidBrush(Color.FromArgb(255, 33, 35, 38)))
                    e.Graphics.FillRectangle(bgBrush, scrollBgX, scrollBgY, scrollBgWidth, scrollBgHeight);

                // Draw the scrollbar thumb
                using (var thumbBrush = new SolidBrush(Color.White))
                    e.Graphics.FillRectangle(thumbBrush, scrollBgX + thumbMargin, thumbPos + thumbMargin, thumbWidth, thumbHeight - (thumbMargin * 2));
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            //base.OnMouseWheel(e);

            int scrollAmount = e.Delta;//SystemInformation.MouseWheelScrollLines * e.Delta;
            int maxScroll = panelContentsHeight - scrollBgHeight;
            scrollPosition = Math.Max(0, Math.Min(scrollPosition - scrollAmount, maxScroll));

            // Force a repaint
            PerformScroll();

            // Workaround for small scrolls not updating the child panels even when invalidating or refreshing
            OnVisibleChanged(EventArgs.Empty);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            /*if (e.Button == MouseButtons.Left) {
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
            }*/
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            /*if (thumbDragging) {
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
            }*/
        }

        


        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left) {
                // Stop thumb dragging
                thumbDragging = false;
            }
        }

        private int ContentHeight() {
            //return 20 + (30 * 16);

            int totalHeight = 0;
            foreach (Control control in Controls)
                if (control.Parent == this)
                    totalHeight += control.Height;

            return totalHeight;
        }
    }
}
