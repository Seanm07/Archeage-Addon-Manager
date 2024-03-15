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

        private int thumbMargin = 4;
        private float scrollPosition = 0f;
        private bool thumbDragging = false;
        private int thumbDragOffset = 0;

        private int scrollBgX, scrollBgY, scrollBgWidth, scrollBgHeight;
        private int panelContentsHeight, thumbWidth, thumbHeight, thumbPos;

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public CustomPanel() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            this.Layout += LayoutUpdate;
        }

        // Called once the layout is ready and heights are no longer 0
        private void LayoutUpdate(object? sender, LayoutEventArgs e) {
            panelContentsHeight = ContentHeight();

            scrollBgWidth = SystemInformation.VerticalScrollBarWidth;
            scrollBgHeight = Height;
            scrollBgX = Width - scrollBgWidth;
            scrollBgY = 0;

            thumbWidth = scrollBgWidth - (thumbMargin * 2);
            thumbHeight = (int)((float)scrollBgHeight * ((float)scrollBgHeight / (float)panelContentsHeight));

            // This needs to be called again when releasing the scrollbar otherwise the windows scrollbar blocks the custom scrollbar again
            _ = ShowScrollBar(Handle, 1, 0); // ID 1 is vertical scrollbar

            AutoScroll = true;
            VerticalScroll.Visible = false;
        }

        protected override void OnSizeChanged(EventArgs e) {
            // This needs to be called again when releasing the scrollbar otherwise the windows scrollbar blocks the custom scrollbar again
            _ = ShowScrollBar(Handle, 1, 0); // ID 1 is vertical scrollbar

            AutoScroll = false;
        }

        private void PerformScroll() {
            thumbHeight = (int)((float)scrollBgHeight * ((float)scrollBgHeight / (float)panelContentsHeight));
            thumbPos = (int)((float)scrollPosition * ((float)scrollBgHeight - (float)thumbHeight) / ((float)panelContentsHeight - (float)scrollBgHeight));

            // Calculate scroll percentage
            float scrollPercentage = (float)scrollPosition / ((float)panelContentsHeight - (float)thumbHeight);
            int scrollY = (int)(scrollPercentage * VerticalScroll.Maximum);

            AutoScrollPosition = new Point(0, scrollY);
            VerticalScroll.Value = scrollY;
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (panelContentsHeight > thumbHeight) {
                // Draw the scrollbar background
                using (var bgBrush = new SolidBrush(Color.FromArgb(255, 33, 35, 38)))
                    e.Graphics.FillRectangle(bgBrush, scrollBgX, scrollBgY, scrollBgWidth, scrollBgHeight);

                // Draw the scrollbar thumb
                using (var thumbBrush = new SolidBrush(Color.White))
                    e.Graphics.FillRectangle(thumbBrush, scrollBgX + thumbMargin, thumbPos + thumbMargin, thumbWidth, thumbHeight - (thumbMargin * 2));
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            
            float scrollChange = e.Delta * 0.5f;

            scrollPosition = Math.Max(0f, Math.Min(scrollPosition - scrollChange, panelContentsHeight - scrollBgHeight));

            // Force a repaint
            PerformScroll();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left) {
                // Get the scroll bounds of the thumb and store it in a rectangle
                Rectangle thumbBounds = new Rectangle(scrollBgX, thumbPos, scrollBgWidth, thumbHeight);

                // Check if the mouse is within the thumb bounds
                if (thumbBounds.Contains(e.Location)) {
                    // Start dragging the thumb
                    thumbDragging = true;
                    thumbDragOffset = e.Y - thumbBounds.Top;
                }

                PerformScroll();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left) {
                thumbDragging = false;

                PerformScroll();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (thumbDragging) {
                // Calculate new scroll position based on thumb drag
                scrollPosition = Math.Max(0f, Math.Min((((float)(e.Y - thumbDragOffset) / (float)(scrollBgHeight - thumbHeight)) * (float)(panelContentsHeight - scrollBgHeight)), panelContentsHeight - scrollBgHeight));

                // Force a repaint
                PerformScroll();
            }
        }

        private int ContentHeight() {
            int totalHeight = 0;

            foreach (Control control in Controls)
                if (control.Parent == this)
                    totalHeight += control.Height;

            return totalHeight;
        }
    }
}
