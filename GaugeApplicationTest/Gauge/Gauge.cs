using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;

namespace GaugeControl
{
    public class Gauge : UserControl
    {
        /* private variables */
        private Boolean       drawGaugeBackground = true;
        private Boolean       isGridEnabled = false;

        /* protected variables */
        protected float       m_angle = 0.0f;
        protected Decimal     m_value = 0;
        protected Bitmap      gaugeBitmap;
        protected PointF      CenterPoint;
 
        protected List<GaugeMarker> m_markerList = new List<GaugeMarker>();
        protected Dictionary<string, GaugeNeedle> customNeedleDictionary = new Dictionary<string, GaugeNeedle>();

        protected SimpleGaugeNeedle m_simpleNeedle = new SimpleGaugeNeedle(Color.Red, new Size(4,66), Color.Chocolate, 10);

        /* TODO : Should make arc a gaugeelement. */
        protected Boolean     m_isArcEnabled =                true;
        protected int         m_arcWidth =                    3;
        protected float       m_arcStartAngle =               180f;
        protected float       m_arcEndAngle =                 360f;
        protected float       m_arcRadius =                   70;
        protected Color       m_arcColor =                    Color.White;
        

        /* Main number markers are bound to the arc radius with this variable. */
        protected int         m_NumberMarkerOffset =          15;

        protected Boolean     m_isBackGroundEllipseEnabled =  true;
        protected Color       m_backGroundEllipseColor =      Color.Black;
        protected float       m_backGroundEllipseRadius =     90;

        protected GaugeBorder m_Border;
        protected Boolean     m_isBorderEnabled =             true;
        protected Boolean     m_isCustomNeedleEnabled =       false; /* Enabled needle from bitmap. */

        protected Bitmap      m_needleBitMap;
        protected Bitmap      ResizedNeedleBitMap;
        protected float       ResizeScale = 70; /* Really should review this. */
        protected PointF      NeedleCenterPoint = new PointF(20f, 40f);
        
        /* public variables. */
        public GaugeNeedle mainNeedle;
        public GaugeNumberMarker mainNumberMarker;


        /* Declarations needed for collections. */
        public delegate void ItemAddedEventHandler();

        public class CustomCollection<T> : Collection<T> where T : GaugeElement
        {
            public ItemAddedEventHandler myHandler;

            public CustomCollection(ItemAddedEventHandler handler)
            {
                myHandler = handler;
            }

            protected override void InsertItem(int index, T item)
            {
                //item.Name = item.GetType().Name.ToString() + " " + index;
                base.InsertItem(index, item);
                myHandler();
            }
        }

        #region properties

        [Browsable(true),
        Category("Border"),
        Description("Is border enabled")]
        public Boolean BorderEnabled
        {
            get { return m_isBorderEnabled; }
            set
            {
                if (m_isBorderEnabled != value)
                {
                    m_isBorderEnabled = value;
                    drawGaugeBackground = true;
                }
                Refresh();
            }
        }

        [Browsable(true),
        Category("Border"),
        Description("Border color")]
        public Color BorderColor
        {
            get { return m_Border.BorderColor; }
            set
            {
                if (m_Border.BorderColor != value)
                {
                    m_Border.BorderColor = value;
                    drawGaugeBackground = true;
                }
                Refresh();
            }
        }

        [Browsable(true),
        Category("Border"),
        Description("Border width")]
        public float BorderWidth
        {
            get { return m_Border.BorderWidth; }
            set
            {
                if (m_Border.BorderWidth != value)
                {
                    m_Border.BorderWidth = value;
                    drawGaugeBackground = true;
                }
                Refresh();
            }
        }


        [Browsable(true),
        Category("NumberMarkers"),
        Description("Marker font")]
        public Font NumberMarkerFont
        {
            get { return mainNumberMarker.Font; }
            set
            {
                if (value != null)
                {
                    mainNumberMarker.Font = value;
                    drawGaugeBackground = true;
                    Invalidate();
                }
            }
        }


        [Browsable(true),
        Category("NumberMarkers"),
        Description("Begin Angle")]
        public float NumberMarkerAngleBegin
        {
            get { return mainNumberMarker.BeginAngle; }
            set 
            { 
                mainNumberMarker.BeginAngle = value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Number marker arc end angle")]
        public float NumberMarkerAngleEnd
        {
            get { return mainNumberMarker.EndAngle; }
            set
            {
                mainNumberMarker.EndAngle = value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Angle Interval")]
        public float NumberMarkerAngleInterval
        {
            get { return mainNumberMarker.IntervalAngle; }
            set
            {
                mainNumberMarker.IntervalAngle = value;
                drawGaugeBackground = true;
                Invalidate();
                Refresh();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("First numerical value")]
        public Decimal NumberMarkerValueBegin
        {
            get { return mainNumberMarker.ValueBegin; }
            set
            {
                mainNumberMarker.ValueBegin = value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Value interval ")]
        public Decimal NumberMarkerValueInterval
        {
            get { return mainNumberMarker.ValueInterval; }
            set
            {
                mainNumberMarker.ValueInterval = value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Number marker width ")]
        public Decimal NumberMarkerWidth
        {
            get { return (Decimal)mainNumberMarker.Width; }
            set
            {
                mainNumberMarker.Width = (float)value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Number marker height ")]
        public Decimal NumberMarkerHeight
        {
            get { return (Decimal)mainNumberMarker.Height; }
            set
            {
                mainNumberMarker.Height = (float)value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Number marker offset ")]
        public int NumberMarkerOffset
        {
            get { return m_NumberMarkerOffset; }
            set
            {
                m_NumberMarkerOffset = value;
                mainNumberMarker.Radius = m_arcRadius + m_NumberMarkerOffset;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("NumberMarkers"),
        Description("Number marker offset ")]
        public Color NumberMarkerColor
        {
            get { return mainNumberMarker.markerColor; }
            set
            {
                mainNumberMarker.markerColor = value;
                drawGaugeBackground = true;
                Invalidate();
            }
        }

        [Browsable(true),
        Category("Gauge"),
        Description("The value.")]
        public float Angle
        {
            get { return (int)m_angle; }
        }

        [Browsable(true),
        Category("Arc"),
        Description("Is arc visible")]
        public Boolean isArcEnabled
        {
            get { return m_isArcEnabled; }
            set
            {
                if (m_isArcEnabled != value)
                {
                    m_isArcEnabled = value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Arc"),
        Description("Arc width")]
        public Single ArcWidth
        {
            get { return m_arcWidth; }
            set
            {
                if (m_arcWidth != value)
                {
                    m_arcWidth = (int)value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Arc"),
        Description("Arc start angle")]
        public float ArcStartAngle
        {
            get { return m_arcStartAngle; }
            set
            {
                if (m_arcStartAngle != value)
                {
                    m_arcStartAngle = (int)value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Arc"),
        Description("Arc end angle")]
        public float ArcEndAngle
        {
            get { return m_arcEndAngle; }
            set
            {
                if (m_arcEndAngle != value)
                {
                    m_arcEndAngle = (int)value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Arc"),
        Description("Arc Radius")]
        public float ArcRadius
        {
            get { return m_arcRadius; }
            set
            {
                if (m_arcRadius != value)
                {
                    m_arcRadius = value;
                    drawGaugeBackground = true;
                    mainNumberMarker.Radius = m_arcRadius + m_NumberMarkerOffset;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Arc"),
        Description("Arc Color")]
        public Color ArcColor
        {
            get { return m_arcColor; }
            set
            {
                if (m_arcColor != value)
                {
                    m_arcColor = value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Needle"),
        Description("Enable custom needle")]
        public Boolean isCustomNeedleEnabled
        {
            get { return m_isCustomNeedleEnabled; }
            set 
            {
                m_isCustomNeedleEnabled = value;
                UpdateNeedleBitmap();
                Refresh();
            }
            
        }

        [Browsable(true),
        Category("Needle"),
        Description("Simple needle size")]
        public Size NeedleSize
        {
            get { return m_simpleNeedle.needleSize; }
            set 
            { 
                m_simpleNeedle.needleSize = value;
                UpdateNeedleBitmap();
            }
        }

        [Browsable(true),
        Category("Needle"),
        Description("Simple needle color")]
        public Color NeedleColor
        {
            get { return m_simpleNeedle.NeedleColor; }
            set 
            { 
                m_simpleNeedle.NeedleColor = value;
                UpdateNeedleBitmap();
            }
        }

        [Browsable(true),
        Category("Needle"),
        Description("Simple needle center radius")]
        public float NeedleCenterRadius
        {
            get { return m_simpleNeedle.CenterRadius; }
            set 
            { 
                m_simpleNeedle.CenterRadius = value;
                UpdateNeedleBitmap();
            }
        }

        [Browsable(true),
        Category("Needle"),
        Description("Simple needle center color")]
        public Color NeedleCenterColor
        {
            get { return m_simpleNeedle.CenterColor; }
            set 
            { 
                m_simpleNeedle.CenterColor = value;
                UpdateNeedleBitmap();
            }
        }

        [Browsable(true),
        Category("Gauge"),
        Description("Gauge background ellipse color")]
        public Color BackGroundEllipseColor
        {
            get { return m_backGroundEllipseColor; }
            set
            {
                if (m_backGroundEllipseColor != value)
                {
                    m_backGroundEllipseColor = value;
                    drawGaugeBackground = true;
                    Refresh();
                }
            }
        }

        [Browsable(true),
        Category("Needle"),
        Description("Gauge needle image")]
        public Image NeedleImage
        {
            get { return m_needleBitMap; }
            set
            {
                m_needleBitMap = (Bitmap)value;
                UpdateNeedleBitmap();
            }
        }

        [Browsable(true),
        Category("Logical Gauge Value"),
        Description("Gauge logical value, corresponding angle is defined by number scale")]
        public Decimal Value
        {
            get { return m_value; }
            set
            {
                SetValue(value);
            }
        }


        [Browsable(true),
        Category("Logical Gauge Value"),
        Description("Gauge needle image")]
        public Decimal MaxValue { get; set; } = 1000; /* TODO : Take this into account. */


        [Browsable(true),
        Category("Logical Gauge Value"),
        Description("Gauge needle image")]
        public Decimal MinValue { get; set; } = 0; /* TODO : Take this into account. */


        private CustomCollection<GaugeTickMarker> m_TickMarkerCollection;

        [Category("Test")]
        [Description("The collection fo the tick markers on the gauge. ")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CustomCollection<GaugeTickMarker> TickMarkers
        {
            get { return m_TickMarkerCollection; }
        }

        private CustomCollection<GaugeLabel> m_CustomTextCollection;

        [Category("Test")]
        [Description("The collection for labels on the gauge. ")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CustomCollection<GaugeLabel> Labels
        {
            get { return m_CustomTextCollection; }
        }

        /* TODO : Add collections for number markers and also custom needles. */


        /*********************************************************************************/

        #endregion


        public Gauge()
        {
            AutoScaleMode = AutoScaleMode.None;
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.CenterPoint = new PointF(Width / 2, Height / 2);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;

            m_Border = new GaugeBorder();
            m_Border.BorderRadius = m_backGroundEllipseRadius;

            mainNumberMarker = new GaugeNumberMarker(m_arcRadius + m_NumberMarkerOffset);

            UpdateNeedleBitmap();

            /* This is under test. */
            m_TickMarkerCollection = new CustomCollection<GaugeTickMarker>(new ItemAddedEventHandler(UpdateBackGround));
            m_CustomTextCollection = new CustomCollection<GaugeLabel>(new ItemAddedEventHandler(UpdateBackGround));

            /* Set size to default values. */
            this.Size = new Size(220, 220);
        }

        /* Can be overwritten, called from property change. */
        public void SetValue(Decimal value)
        {
            Decimal begin = mainNumberMarker.ValueBegin;

            /* We don't check for value constraints because gauge might have areas where there are no numbers. */
            /* TODO : Should still define purely logical max and min value.*/

            m_value = value;
            float angle_per_value = mainNumberMarker.IntervalAngle / (float)mainNumberMarker.ValueInterval;
            Decimal relative_value = m_value - begin;

            SetAngle((angle_per_value * (float)relative_value) + mainNumberMarker.BeginAngle);
        }

        public void AddMarker(GaugeMarker m)
        {
            this.m_markerList.Add(m);
            drawGaugeBackground = true;
            this.Refresh();
        }

        public void AddGaugeLabel(GaugeLabel t)
        {
            this.m_CustomTextCollection.Add(t);
            drawGaugeBackground = true;
            this.Refresh();
        }

        public void AddCustomNeedle(string name, GaugeNeedle needle)
        {
            customNeedleDictionary.Add(name, needle);
        }

        public void SetCustomNeedleAngle(string name, float angle)
        {
            if (customNeedleDictionary.ContainsKey(name))
            {
                customNeedleDictionary[name].Angle = angle;
                this.Invalidate();
            }
        }


        private void UpdateNeedleBitmap()
        {
            if (m_needleBitMap != null && m_isCustomNeedleEnabled)
            {
                /* We use bitmap needle.*/
                /* TODO : This probably no longer works and should be reviewed. */
                int ResizedWidth = (int)(m_needleBitMap.Width * (ResizeScale / 100));
                int ResizedHeight = (int)(m_needleBitMap.Height * (ResizeScale / 100));

                ResizedNeedleBitMap = new Bitmap((int)Width, (int)Height);

                Graphics graph = Graphics.FromImage(ResizedNeedleBitMap);

                graph.DrawImage(m_needleBitMap, new RectangleF((Width / 2) - NeedleCenterPoint.X, (Height / 2) - NeedleCenterPoint.Y, ResizedWidth, ResizedHeight));
                mainNeedle = new ImageGaugeNeedle(ResizedNeedleBitMap, CenterPoint);
            }
            else
            {
                /* We use the default needle. */
                mainNeedle = m_simpleNeedle;
            }
            Refresh();
        }


        protected override void OnResize(EventArgs e)
        {
            drawGaugeBackground = true;
            this.CenterPoint = new PointF(Width / 2, Height / 2);

            m_backGroundEllipseRadius = Math.Min(this.Size.Height, this.Size.Width) / 2;
            m_Border.BorderRadius = m_backGroundEllipseRadius;

            UpdateNeedleBitmap();

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(ClientRectangle);

            Invalidate(Region);
            Region = new Region(path);

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            /* No point in drawing anything if size is so small */
            if ((Width < 10) || (Height < 10))
            {
                return;
            }

            if (drawGaugeBackground)
            {
                /* Draw background */
                gaugeBitmap = new Bitmap(Width, Height, pe.Graphics);
                Graphics ggr = Graphics.FromImage(gaugeBitmap);
                /* This line might cause background problems? */
                ggr.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

                drawBackGround(ggr);
                drawGaugeBackground = false;

                /* The numeric scale might have changed. */
                Decimal val = Value;
                SetValue(val);
            }
            
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            pe.Graphics.DrawImageUnscaled(gaugeBitmap, 0, 0);

            //Draw custom needles
            foreach (KeyValuePair<string, GaugeNeedle> pair in customNeedleDictionary)
            {
                pair.Value.Draw(pe.Graphics, this.CenterPoint);
            }

            //Draw primary needle. Currently it is set on top.
            mainNeedle.Angle = m_angle;
            mainNeedle.Draw(pe.Graphics, this.CenterPoint);
        }

        private void drawBackGround(Graphics gfx)
        {
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

            Brush myBrush;
            Pen myPen;

            /* TODO : Arc and background Ellipse should be made into gaugeElements. */
            /* Draw outer border. */
            if (m_isBorderEnabled)
            {
                m_Border.Draw(gfx, this.CenterPoint);
            }

            /* If enabled, draw background ellipse. */
            if (m_isBackGroundEllipseEnabled)
            {
                float radius = m_backGroundEllipseRadius;

                if (m_isBorderEnabled)
                {
                    radius -= m_Border.BorderWidth;
                }
                if (radius > 0)
                {
                    RectangleF rect = new RectangleF(CenterPoint.X - radius, CenterPoint.Y - radius, 2 * radius, 2 * radius);
                    myBrush = new SolidBrush(m_backGroundEllipseColor);
                    gfx.FillEllipse(myBrush, rect);

                    if (isGridEnabled)
                    {
                        //Draw helper grid.
                        myPen = new Pen(Color.Red);
                        gfx.DrawLine(myPen, new Point((int)CenterPoint.X, 0), new Point((int)CenterPoint.X, Height));
                        gfx.DrawLine(myPen, new Point(0, (int)CenterPoint.Y), new Point(Width, (int)CenterPoint.Y));
                    }
                }
            }

            //Draw markers
            foreach (GaugeMarker m in m_markerList)
            {
                m.Draw(gfx, this.CenterPoint);
            }
            
            //Draw tick marker collection. 
            foreach (GaugeTickMarker m in m_TickMarkerCollection)
            {
                m.Draw(gfx, this.CenterPoint);
            }
            
            //Draw default number marker.
            mainNumberMarker.Draw(gfx, this.CenterPoint);

            /* Draw the arc */
            if (m_isArcEnabled)
            {
                if (m_arcRadius > 0)
                {
                    myPen = new Pen(m_arcColor, m_arcWidth);
                    gfx.DrawArc(myPen, new RectangleF(CenterPoint.X - m_arcRadius, CenterPoint.Y - m_arcRadius, 2 * m_arcRadius, 2 * m_arcRadius), m_arcStartAngle, (m_arcEndAngle - m_arcStartAngle));
                }
            }

            
            //Draw custom text fields.
            foreach(GaugeLabel label in m_CustomTextCollection)
            {
                label.Draw(gfx, this.CenterPoint);
            }
        }

        public void SetAngle(float angle)
        {
            this.m_angle = angle;
            this.Invalidate();
        }

        public float GetAngle()
        {
            return this.m_angle;
        }

        private void UpdateBackGround()
        {
            drawGaugeBackground = true;
            this.Invalidate();
        }
    }
}

