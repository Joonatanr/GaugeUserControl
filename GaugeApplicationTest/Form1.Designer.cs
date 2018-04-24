namespace GaugeApplicationTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gauge1 = new GaugeControl.Gauge();
            this.SuspendLayout();
            // 
            // gauge1
            // 
            this.gauge1.ArcColor = System.Drawing.Color.White;
            this.gauge1.ArcEndAngle = 360F;
            this.gauge1.ArcRadius = 80F;
            this.gauge1.ArcStartAngle = 180F;
            this.gauge1.ArcWidth = 3F;
            this.gauge1.BackColor = System.Drawing.Color.Transparent;
            this.gauge1.BackGroundEllipseColor = System.Drawing.Color.Black;
            this.gauge1.BorderColor = System.Drawing.Color.Black;
            this.gauge1.BorderEnabled = true;
            this.gauge1.BorderWidth = 4F;
            this.gauge1.isArcEnabled = true;
            this.gauge1.isCustomNeedleEnabled = false;
            this.gauge1.Location = new System.Drawing.Point(112, 147);
            this.gauge1.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.gauge1.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.gauge1.Name = "gauge1";
            this.gauge1.NeedleCenterColor = System.Drawing.Color.Chocolate;
            this.gauge1.NeedleCenterRadius = 10F;
            this.gauge1.NeedleColor = System.Drawing.Color.Red;
            this.gauge1.NeedleImage = null;
            this.gauge1.NeedleSize = new System.Drawing.Size(4, 66);
            this.gauge1.NumberMarkerAngleBegin = 180F;
            this.gauge1.NumberMarkerAngleEnd = 360F;
            this.gauge1.NumberMarkerAngleInterval = 18F;
            this.gauge1.NumberMarkerColor = System.Drawing.Color.White;
            this.gauge1.NumberMarkerFont = new System.Drawing.Font("Tahoma", 10F);
            this.gauge1.NumberMarkerHeight = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gauge1.NumberMarkerOffset = 20;
            this.gauge1.NumberMarkerValueBegin = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.gauge1.NumberMarkerValueInterval = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gauge1.NumberMarkerWidth = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gauge1.Size = new System.Drawing.Size(268, 246);
            this.gauge1.TabIndex = 0;
            this.gauge1.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gauge1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private GaugeControl.Gauge gauge1;
    }
}

