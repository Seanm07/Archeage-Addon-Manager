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
            Refresh();
            Refresh();
        }
    }

    public class ToolstripRenderer : ToolStripProfessionalRenderer {

        // Override the system default menu item background rendering
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;

            // Setup the bounds of our item to have a 2 pixel border around it
            Rectangle bounds = new Rectangle(new Point(2, 0), new Size(item.Width - 3, item.Height));

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
            // Force all toolstrip text to be white
            e.TextColor = Color.White;

            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
            ToolStripItem item = e.Item;

            // If the tooltip item has an image assigned, render it to the left of the label
            if (item.Image != null)
                e.Graphics.DrawImage(item.Image, new Rectangle(13, 4, 13, 13));
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            // Disable rendering of toolstrip borders (removes shadow on toolstrip dropdown menu items)
            //base.OnRenderToolStripBorder(e);
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
            // Without double buffering the scrollbar flickers turning transparent when scrolling
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            this.Layout += LayoutUpdate;
        }

        // Called when inner layout updates (before this is initially called ContentHeight() will return 0)
        private void LayoutUpdate(object? sender, LayoutEventArgs e) {
            panelContentsHeight = ContentHeight();

            scrollBgWidth = SystemInformation.VerticalScrollBarWidth;
            scrollBgHeight = Height;
            scrollBgX = Width - scrollBgWidth;
            scrollBgY = 0;

            thumbWidth = scrollBgWidth - (thumbMargin * 2);
            thumbHeight = (int)((float)scrollBgHeight * ((float)scrollBgHeight / (float)panelContentsHeight));

            // Temporarily enable autoscroll to force the contents to follow the scroll value
            AutoScroll = true;

            // Force hide the windows default scrollbar
            _ = ShowScrollBar(Handle, 1, 0); // ID 1 is vertical scrollbar

            // Hide the vertical scrollbar
            VerticalScroll.Visible = false;
        }

        protected override void OnSizeChanged(EventArgs e) {
            // Force hide the windows default scrollbar when a size change event happens 
            _ = ShowScrollBar(Handle, 1, 0); // ID 1 is vertical scrollbar

            // On size change event disable autoscroll to fix a bug with the windows default scrollbar flickering
            AutoScroll = false;
        }

        private void PerformScroll() {
            thumbHeight = (int)((float)scrollBgHeight * ((float)scrollBgHeight / (float)panelContentsHeight));
            thumbPos = (int)((float)scrollPosition * ((float)scrollBgHeight - (float)thumbHeight) / ((float)panelContentsHeight - (float)scrollBgHeight));

            // Calculate scroll percentage
            float scrollPercentage = (float)scrollPosition / ((float)panelContentsHeight - (float)thumbHeight);
            int scrollY = (int)(scrollPercentage * VerticalScroll.Maximum);

            // Clamp minimum value at 1 otherwise it doesn't bother updating the scroll when setting it to 0
            VerticalScroll.Value = Math.Max(scrollY, 1);

            // Force refresh the panel contents to update to the current scroll value
            Refresh();
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
            
            // The default scroll delta was too fast so this reduces it
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

                    // Keep track of where the cursor is relative to the thumb bar
                    thumbDragOffset = e.Y - thumbBounds.Top;
                } else {
                    // Check if the user clicked somewhere else on the bar background
                    Rectangle scrollBgBounds = new Rectangle(scrollBgX, scrollBgY, scrollBgWidth, scrollBgHeight);

                    if (scrollBgBounds.Contains(e.Location)) {
                        // Jump the scroll position to the clicked position
                        float minScroll = 0f;
                        float maxScroll = panelContentsHeight - scrollBgHeight;
                        float scrollAmount = (e.Location.Y - (thumbHeight / 2)) / (float)(scrollBgHeight - thumbHeight) * maxScroll;

                        // Calculate new scroll position based on thumb drag
                        scrollPosition = Math.Max(minScroll, Math.Min(scrollAmount, maxScroll));

                        // Sync the scroll to the current dragged scroll bar
                        PerformScroll();
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left) {
                thumbDragging = false;

                // Force sync the scroll on mouse click release to be safe
                PerformScroll();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (thumbDragging) {
                float minScroll = 0f;
                float maxScroll = panelContentsHeight - scrollBgHeight;
                float scrollAmount = (e.Y - thumbDragOffset) / (float)(scrollBgHeight - thumbHeight) * maxScroll;

                // Calculate new scroll position based on thumb drag
                scrollPosition = Math.Max(minScroll, Math.Min(scrollAmount, maxScroll));

                // Sync the scroll to the current dragged scroll bar
                PerformScroll();
            }
        }

        private int ContentHeight() {
            int totalHeight = 0;

            // Sum the height of all panels in the root to get content size for scrollbar calculations
            foreach (Control control in Controls)
                if (control.Parent == this)
                    totalHeight += control.Height;

            return totalHeight;
        }
    }
}
