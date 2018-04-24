using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GaugeControl
{
    public abstract class GaugeNeedle : GaugeElement
    {
        private Boolean m_isGridEnabled = false;

        protected float     x_offset = 0;
        protected float     y_offset = 0;
        
        public Size needleSize  { get; set; }
        public int Priority     { get; set; }
        public float Angle      { get; set; }
        
        public Boolean isGridEnabled
        {
            get { return m_isGridEnabled; }
            set { m_isGridEnabled = value; }
        }

        public float xOffset { get { return x_offset; } set { x_offset = value; } }
        public float yOffset { get { return y_offset; } set { y_offset = value; } }

        public GaugeNeedle()
        {
            this.Priority = 10;
        }
        
        public virtual void SetOffset(PointF offset)
        {
            this.Offset = offset;
        }
        

        public GaugeNeedle(PointF center)
        {
            this.Priority = 10;
            xOffset = 0;
            yOffset = 0;
        }

        public GaugeNeedle(PointF center, int prio)
        {
            this.Priority = prio;
        }


        public static Bitmap RotateImage(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            int ImageSize = Math.Max(image.Width, image.Height);

            Bitmap rotatedBmp = new Bitmap(ImageSize * 2, ImageSize * 2);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            //This is required to prevent memory usage from skyrocketing.
            g.Dispose();

            return rotatedBmp;
        }
    }


    public class SimpleGaugeNeedle : GaugeNeedle
    {
        public Color CenterColor {get; set;}
        public Color NeedleColor {get; set;}
        public float CenterRadius {get; set;}

        public enum NeedleShape{
            NeedleTriangle,
            NeedleRectangle,
        }

        private NeedleShape _shape = NeedleShape.NeedleTriangle;
        //private NeedleShape _shape = NeedleShape.NeedleRectangle;

        public NeedleShape Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        public SimpleGaugeNeedle(Color myColor, Size mySize, Color centerColor, float center_radius)
        {
            this.Priority = 10;
            this.CenterColor = centerColor;
            this.NeedleColor = myColor;
            this.CenterRadius = center_radius;
            this.Angle = 0;

            this.needleSize = mySize;
        }

        public SimpleGaugeNeedle()
        {
            this.Priority = 10;
            this.CenterColor = Color.Red;
            this.NeedleColor = Color.Red;
            this.CenterRadius = 15;
            this.Angle = 0;
            this.needleSize = new Size(4, 66); //Choose some arbitrary value.
        }


        protected override void DrawElement(Graphics g)
        {
            //Draw simple gauge needle.
            Brush myBrush;

            PointF CenterOffset = new PointF(m_centerPoint.X + x_offset, m_centerPoint.Y + y_offset);

            switch (_shape)
            {
                case NeedleShape.NeedleRectangle:

                    float myAngle = ((this.Angle * (float)Math.PI) / 180f);
                    float xPos = CenterOffset.X - ((this.needleSize.Height) * (float)(Math.Cos(myAngle)));
                    float yPos = CenterOffset.Y - ((this.needleSize.Height) * (float)(Math.Sin(myAngle)));
                    Pen myPen = new Pen(this.NeedleColor, this.needleSize.Width);
                    g.DrawLine(myPen, CenterOffset, new PointF(xPos,yPos));

                    break;
                case NeedleShape.NeedleTriangle:
                    
                    PointF point1 = new PointF(CenterOffset.X - needleSize.Width / 2, CenterOffset.Y);
                    PointF point2 = new PointF(CenterOffset.X + needleSize.Width / 2, CenterOffset.Y);
                    PointF point3 = new PointF(CenterOffset.X + 0.5f, CenterOffset.Y - needleSize.Height);
                    PointF point4 = new PointF(CenterOffset.X - 0.5f, CenterOffset.Y - needleSize.Height);

                    PointF[] curvePoints = { point1, point2, point3, point4 };
                    
                    myBrush = new SolidBrush(NeedleColor);
                    DrawRotatedPolygon(g, curvePoints, Angle + 90, myBrush, CenterOffset);
                    break;
            }

            /* Draw center */
            if (CenterRadius > 0)
            {
                myBrush = new SolidBrush(this.CenterColor);   
                RectangleF r2 = new RectangleF(CenterOffset.X - (CenterRadius / 2),
                                               CenterOffset.Y - (CenterRadius / 2),
                                               CenterRadius,
                                               CenterRadius);
                g.FillEllipse(myBrush, r2);
            }
        }

        private void DrawRotatedPolygon(Graphics g, PointF [] points, float angle, Brush ColorBrush, PointF RotPoint)
        {
            using (Matrix m = new Matrix())
            {
               // m.Translate(x_offset, y_offset);
                m.RotateAt(angle, RotPoint); 
                g.Transform = m;

                // Define fill mode.
                FillMode newFillMode = FillMode.Winding;
                // Fill polygon to screen.
                g.FillPolygon(ColorBrush, points, newFillMode);

                if (isGridEnabled)
                {
                    Pen myPen = new Pen(Color.Blue);

                    g.DrawLine(myPen, new Point((int)m_centerPoint.X, 0), new PointF((int)m_centerPoint.X, m_centerPoint.Y * 2));
                    g.DrawLine(myPen, new Point(0, (int)m_centerPoint.Y), new PointF(m_centerPoint.X * 2, (int)m_centerPoint.Y));
                }
                g.ResetTransform();
            }
        }
    }


    public class ImageGaugeNeedle : GaugeNeedle
    {
        private Bitmap NeedleBitMap;

        public ImageGaugeNeedle(Bitmap bitmap, PointF center)
        {
            this.Offset = center;
            NeedleBitMap = bitmap;
        }
        
        protected override void DrawElement(Graphics g)
        {
            //Draw needle.
            using (Image RotatedImage = RotateImage(NeedleBitMap, m_centerPoint, Angle))
            {
                g.DrawImage(RotatedImage, x_offset, y_offset);
            }
        }
    }

    public class DialGaugeNeedle : GaugeNeedle
    {
        public float DialRadius { get; set; }
        public Color DialColor { get; set; }
        public PointF[] CroppingPoints;

        private GaugeNumberMarker m_numberMarker = null;
        private GaugeTickMarker m_tickMarker = null;

        private Bitmap DialBitmap;

        public override void SetOffset(PointF offset)
        {
            base.SetOffset(offset);
            this.UpdateDialBitmap();
        }

        public GaugeNumberMarker NumberMarker
        {
            get { return m_numberMarker; }
            set
            {
                m_numberMarker = value;
                this.UpdateDialBitmap();
            }
        }

        public GaugeTickMarker TickMarker
        {
            get { return m_tickMarker; }
            set
            {
                m_tickMarker = value;
                this.UpdateDialBitmap();
            }
        }

        public DialGaugeNeedle()
        {
            this.DialRadius = 25;
            UpdateDialBitmap();
        }

        public DialGaugeNeedle(float radius, Color dialColor)
        {
            this.DialRadius = radius;
            this.DialColor = dialColor;

            UpdateDialBitmap();
        }

        private void UpdateDialBitmap()
        {
            //Generate new bitmap for dial. Should only be called when some property of the dial is changed.
            DialBitmap = new Bitmap((int)DialRadius * 2, (int)DialRadius * 2);
            Graphics g = Graphics.FromImage(DialBitmap);
            RectangleF rect = new RectangleF(0, 0, DialRadius * 2, DialRadius * 2);

            Brush myBrush = new SolidBrush(DialColor);

            g.FillEllipse(myBrush, rect);

            //Draw Grid
            if (isGridEnabled)
            {
                Pen myPen = new Pen(Color.Black);

                g.DrawLine(myPen, new Point((int)DialRadius, 0), new PointF((int)DialRadius, DialRadius * 2));
                g.DrawLine(myPen, new Point(0, (int)DialRadius), new PointF(DialRadius * 2, (int)DialRadius));
            }
            if (m_numberMarker != null)
            {
                // Draw number markers for gauge.
                m_numberMarker.Offset = new PointF(DialRadius, DialRadius);
                m_numberMarker.Draw(g, this.m_centerPoint);
            }

            if (m_tickMarker != null)
            {
                m_tickMarker.Offset = new PointF(DialRadius, DialRadius);
                m_tickMarker.Draw(g, this.m_centerPoint);
            }
        }

        protected override void DrawElement(Graphics g)
        {
            //Draw dial.
            using (Image RotatedImage = RotateImage(DialBitmap, new PointF(DialRadius, DialRadius), Angle))
            {
                if (CroppingPoints != null)
                {
                    GraphicsPath gp = new GraphicsPath();   // a Graphicspath
                    using (Matrix m = new Matrix())
                    {
                        m.Translate(m_centerPoint.X, m_centerPoint.Y);
                        gp.AddPolygon(CroppingPoints.ToArray());        // with one Polygon
                        gp.Transform(m);
                        g.Clip = new Region(gp);

                        g.DrawImage(RotatedImage, m_centerPoint.X - this.DialRadius + x_offset, m_centerPoint.Y - this.DialRadius + y_offset);
                        g.ResetClip();

                        /* Draw simple border */
                        Pen myPen = new Pen(Color.DarkGray);
                        myPen.Width = 2;
                        g.DrawPolygon(myPen, gp.PathPoints);
                    }
                }
                else
                {
                    g.DrawImage(RotatedImage, m_centerPoint.X - this.DialRadius + x_offset, m_centerPoint.Y - this.DialRadius + y_offset);
                }
            }
        }
    }
}
