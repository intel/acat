namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    partial class TimedGraphDisplayerForm1x2
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
            this.components = new System.ComponentModel.Container();
            this.graphControlRight = new ZedGraph.ZedGraphControl();
            this.graphControlLeft = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // graphControlRight
            // 
            this.graphControlRight.Location = new System.Drawing.Point(464, 28);
            this.graphControlRight.Name = "graphControlRight";
            this.graphControlRight.ScrollGrace = 0D;
            this.graphControlRight.ScrollMaxX = 0D;
            this.graphControlRight.ScrollMaxY = 0D;
            this.graphControlRight.ScrollMaxY2 = 0D;
            this.graphControlRight.ScrollMinX = 0D;
            this.graphControlRight.ScrollMinY = 0D;
            this.graphControlRight.ScrollMinY2 = 0D;
            this.graphControlRight.Size = new System.Drawing.Size(400, 400);
            this.graphControlRight.TabIndex = 3;
            // 
            // graphControlLeft
            // 
            this.graphControlLeft.Location = new System.Drawing.Point(27, 28);
            this.graphControlLeft.Name = "graphControlLeft";
            this.graphControlLeft.ScrollGrace = 0D;
            this.graphControlLeft.ScrollMaxX = 0D;
            this.graphControlLeft.ScrollMaxY = 0D;
            this.graphControlLeft.ScrollMaxY2 = 0D;
            this.graphControlLeft.ScrollMinX = 0D;
            this.graphControlLeft.ScrollMinY = 0D;
            this.graphControlLeft.ScrollMinY2 = 0D;
            this.graphControlLeft.Size = new System.Drawing.Size(400, 400);
            this.graphControlLeft.TabIndex = 2;
            // 
            // TimedGraphDisplayerForm1x2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 441);
            this.Controls.Add(this.graphControlRight);
            this.Controls.Add(this.graphControlLeft);
            this.Name = "TimedGraphDisplayerForm1x2";
            this.Text = "GraphDisplayerSquareForm";
            this.Load += new System.EventHandler(this.TimedGraphDisplayerForm1x2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public ZedGraph.ZedGraphControl graphControlRight;
        public ZedGraph.ZedGraphControl graphControlLeft;
    }
}