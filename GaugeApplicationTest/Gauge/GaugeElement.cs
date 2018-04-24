using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaugeControl
{
    public abstract class GaugeElement
    {
        public Decimal OffsetX
        {
            get { return (Decimal)Offset.X; }
            set
            {
                Offset.X = (float)value;
            }
        }

        public Decimal OffsetY
        {
            get { return (Decimal)Offset.Y; }
            set
            {
                Offset.Y = (float)value;
            }
        }

        public String Name { get; set; } = "GaugeElement";

        protected PointF    m_centerPoint = new PointF(0,0);

        public PointF Offset = new PointF(0, 0);

        public void Draw(Graphics g, PointF centerPoint)
        {
            m_centerPoint = centerPoint;
            m_centerPoint.X += Offset.X;
            m_centerPoint.Y += Offset.Y;
            DrawElement(g);
        }

        protected abstract void DrawElement(Graphics g);
    }
}
