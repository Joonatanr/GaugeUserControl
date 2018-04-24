using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace GaugeControl
{ 
    public abstract class GaugeMarker : GaugeElement
    {
        protected float     m_beginAngle =  0f;
        protected float     m_endAngle =    360f;
        protected float     m_width =       5;
        protected float     m_height =      10;

        private float       m_interval =    10;
        private Color       m_color =       Color.White;
        protected float     m_radius =      50;

        public float EndAngle       { get { return m_endAngle;      } set { m_endAngle = value;     } }
        public float BeginAngle     { get { return m_beginAngle;    } set { m_beginAngle = value;   } }
        public float Width          { get { return m_width;         } set { m_width = value;        } }
        public float Height         { get { return m_height;        } set { m_height = value;       } }

        public float IntervalAngle  { get { return m_interval;      } set { m_interval = value;     } }
        public Color markerColor    { get { return m_color;         } set { m_color = value;        } }
        public float Radius         { get { return m_radius;        } set { m_radius = value;       } }

        public GaugeMarker()
        {
            /* Default constructor. */
        }

        public GaugeMarker(PointF center, float radius)
        {
            this.m_centerPoint = center;
            this.Radius = radius;
        }

        protected override void DrawElement(Graphics g)
        {
            if (m_interval > 0)
            {
                Brush myBrush = new SolidBrush(this.markerColor);
                for (float x = m_beginAngle + 180; x <= m_endAngle + 180; x += m_interval)
                {
                    float myAngle = ((x * (float)Math.PI) / 180f);

                    float xPos = m_centerPoint.X - (m_radius * (float)(Math.Cos(myAngle)));
                    float yPos = m_centerPoint.Y - (m_radius * (float)(Math.Sin(myAngle)));

                    PointF location = new PointF(xPos, yPos);
                    DrawSingle(g, location, x, myBrush);
                }
            }
        }

        public abstract void DrawSingle(Graphics g, PointF location, float angle, Brush b);
    }
    

    public class GaugeTickMarker : GaugeMarker
    {
        public Boolean isHorizontal = false;
        
        public GaugeTickMarker(float radius, PointF center)
        {
            this.Radius = radius;

            /* Initiate to default values */
            m_beginAngle = 0f;
            m_endAngle = 360f;

            this.Height = 8;
            this.Width = 2;
            this.IntervalAngle = 10;
            this.markerColor = Color.White;
        }

        public GaugeTickMarker(float radius)
        {
            this.Radius = radius;

            /* Initiate to default values */
            this.BeginAngle = 0;
            this.EndAngle = 360;
            this.Height = 8;
            this.Width = 2;
            this.IntervalAngle = 10;
            this.markerColor = Color.White;
        }

        public GaugeTickMarker() : this(50)
        {
        }

        public override void DrawSingle(Graphics g, PointF location, float angle, Brush b)
        {   
            if (isHorizontal)
            {
                /* Currently this does not work. */
                float myAngleStart = (((angle - this.Height) * (float)Math.PI) / 180f);

                float xPosStart = m_centerPoint.X - (m_radius * (float)(Math.Cos(myAngleStart)));
                float yPosStart = m_centerPoint.Y - (m_radius * (float)(Math.Sin(myAngleStart)));

                float myAngleEnd = (((angle + this.Height) * (float)Math.PI) / 180f);

                float xPosEnd = m_centerPoint.X - (m_radius * (float)(Math.Cos(myAngleEnd)));
                float yPosEnd = m_centerPoint.Y - (m_radius * (float)(Math.Sin(myAngleEnd)));

                Pen myPen = new Pen(this.markerColor, this.Width);
                g.DrawLine(myPen, new PointF(xPosStart,yPosStart), new PointF(xPosEnd, yPosEnd));
            }
            else
            {
                float myAngle = ((angle * (float)Math.PI) / 180f);
                float xPos = m_centerPoint.X - ((m_radius - this.Height) * (float)(Math.Cos(myAngle)));
                float yPos = m_centerPoint.Y - ((m_radius - this.Height) * (float)(Math.Sin(myAngle)));
                Pen myPen = new Pen(this.markerColor, this.Width);
                g.DrawLine(myPen, location, new PointF(xPos,yPos));
            }
        }
    }

    public class GaugeNumberMarker : GaugeMarker
    {
        private Decimal m_value_begin = 0;
        private Decimal m_value_end = 100;
        private Decimal m_value_interval = 10;

        private Font m_font = new Font("Tahoma", 10);
        private StringFormat m_format;
        private Decimal curr_value = 0;

        public Boolean RotateNumbers = false;
        public float RotationOffset = 0;
        public Boolean isDirectionReversed = false;


        public Decimal ValueBegin
        {
            get { return m_value_begin; }
            set { m_value_begin = value; }
        }

        public Decimal ValueInterval
        {
            get { return m_value_interval; }
            set { m_value_interval = value; }
        }

        public Font Font
        {
            get { return m_font; }
            set { m_font = value; }
        }

        public decimal ValueEnd
        {
            get { return m_value_end; }
            set { m_value_end = value; }
        }

        public GaugeNumberMarker(float radius)
        {
            this.Radius = radius;
            
            /* Initiate to default values */
            m_beginAngle = 180;
            m_endAngle = 360;
            Height = 10;
            Width = 10;
            IntervalAngle = 18;
            markerColor = Color.White;

            /* Initialize string format to default values. */
            m_format = new StringFormat();
            m_format.Alignment = StringAlignment.Center;
            m_format.LineAlignment = StringAlignment.Center;
        }


        protected override void DrawElement(Graphics g)
        {
            if (this.m_value_interval > 0)
            {
                curr_value = m_value_begin;
                base.DrawElement(g);
            }
        }

        public override void DrawSingle(Graphics g, PointF location, float angle, Brush b)
        {      
            string numString = curr_value.ToString(CultureInfo.CreateSpecificCulture("en-GB")); // output is 1.25)
            SizeF StringSize = g.MeasureString(numString, m_font);

            RectangleF rect = new RectangleF(new PointF(location.X - (StringSize.Width / 2),
                                                          location.Y - StringSize.Height / 2),
                                                          StringSize);
            if (RotateNumbers)
            {
                using (Matrix m = new Matrix())
                {
                    m.RotateAt(angle + RotationOffset, location);
                    g.Transform = m;
                    g.DrawString(numString, m_font, b, rect, this.m_format);
                    g.ResetTransform();
                }
            }
            else
            {
                g.DrawString(numString, m_font, b, rect, this.m_format);
            }
            if (isDirectionReversed)
            {
                curr_value -= this.m_value_interval;
            }
            else
            {
                curr_value += this.m_value_interval;
            }
        }
    }
}