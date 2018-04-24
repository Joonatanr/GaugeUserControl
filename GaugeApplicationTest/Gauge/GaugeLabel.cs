using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaugeControl
{
    public class GaugeLabel : GaugeElement
    {
        public String Text { get; set; } = "";
        public Color TextColor { get; set; } = Color.White;
        public Font  TextFont   { get; set; } = new Font("Arial", 10.0f);

        public GaugeLabel()
        {
            Text = "Text";
        }

        public GaugeLabel (String text) : this()
        {
            this.Text = text;
        }

        protected override void DrawElement(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(TextColor);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            SizeF  t_size = g.MeasureString(Text, TextFont);
            PointF t_point = new PointF(m_centerPoint.X + Offset.X - (t_size.Width / 2), 
                                        m_centerPoint.Y + Offset.Y - (t_size.Height / 2));
            
            RectangleF r1 = new RectangleF(t_point, t_size);

            g.DrawString(Text, TextFont, myBrush, r1, stringFormat);
        }

    }
}
