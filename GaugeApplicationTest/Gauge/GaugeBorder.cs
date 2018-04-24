using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaugeControl
{
    public class GaugeBorder : GaugeElement
    {
        public Color        BorderColor = Color.Black;
        public float        BorderWidth = 4;
        public float        BorderRadius = 10;

        public GaugeBorder()
        {
    
        }

        public GaugeBorder(float width, Color color)
        {
            this.BorderColor = color;
            this.BorderWidth = width;
        }

        protected override void DrawElement(Graphics g)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(m_centerPoint.X - BorderRadius, m_centerPoint.Y - BorderRadius, BorderRadius * 2, BorderRadius * 2);

            // Use the path to construct a brush.
            PathGradientBrush br = new PathGradientBrush(path);

            ColorBlend cb = new ColorBlend();
            cb.Positions = new[] { 0, (BorderWidth * 0.5f) / BorderRadius, BorderWidth / BorderRadius, (2 * BorderWidth) / BorderRadius, 1 };

            cb.Colors = new[] { BorderColor, Color.White, BorderColor, Color.DimGray, Color.DimGray };

            br.InterpolationColors = cb;
            RectangleF myRect = new RectangleF(new PointF(m_centerPoint.X - BorderRadius, m_centerPoint.Y - BorderRadius), new SizeF((BorderRadius) * 2, (BorderRadius) * 2));
            g.FillRectangle(br, myRect);
        }
    }
}
