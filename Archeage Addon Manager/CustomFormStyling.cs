using System.Drawing;
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
}
