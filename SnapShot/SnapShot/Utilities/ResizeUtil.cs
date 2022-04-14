using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot.Utilities
{
    public class ResizeUtil
    {
        public static void resizeControl(Control form,Rectangle formSizeOriginal, Rectangle r, Control c)
        {
            float xRatio = (float)(form.Width) / (float)(formSizeOriginal.Width);
            float yRatio = (float)(form.Height) / (float)(formSizeOriginal.Height);

            int newX = (int)(r.Location.X * xRatio);
            int newY = (int)(r.Location.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);

        }

        public static void resizeFont(Control form, Rectangle formSizeOriginal, Control c, float originalFontSize)
        {

            float newSize = (float)((originalFontSize / (formSizeOriginal.Size.Width + formSizeOriginal.Size.Height)) * (form.Size.Width + form.Size.Height));
            if (newSize > originalFontSize * 1.5)
            {

                c.Font = new Font(c.Font.FontFamily, originalFontSize * 1.5f, c.Font.Style);
                return;
            }
            else if (newSize > originalFontSize)
            {
                c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                return;
            }
            else
            {
                c.Font = new Font(c.Font.FontFamily, originalFontSize, c.Font.Style);
                return;
            }

        }

        public static void resizeFontTypes(Control form,Control c, string type, Rectangle r, float originalFontSize)
        {
            switch (type)
            {
                case "Normal":
                    float newSize = (float)((originalFontSize / (r.Size.Width + r.Size.Height)) * (form.Size.Width + form.Size.Height));
                    if (newSize > originalFontSize * 1.5)
                    {

                        c.Font = new Font(c.Font.FontFamily, originalFontSize * 1.5f, c.Font.Style);
                        return;
                    }
                    else if (newSize > originalFontSize)
                    {
                        c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                        return;
                    }
                    else
                    {
                        c.Font = new Font(c.Font.FontFamily, originalFontSize, c.Font.Style);
                        return;
                    }
                    break;
                case "Inside":
                    newSize = (float)(originalFontSize / (r.Size.Width + r.Size.Height)) * (c.Size.Width + c.Size.Height);
                    if (newSize > originalFontSize * 1.5)
                    {
                        c.Font = new Font(c.Font.FontFamily, originalFontSize * 1.5f, c.Font.Style);
                        return;
                    }
                    else if (newSize > originalFontSize)
                    {
                        c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                        return;
                    }
                    else
                    {
                        c.Font = new Font(c.Font.FontFamily, originalFontSize, c.Font.Style);
                        return;
                    }

                    break;

            }


        }
    }
}
