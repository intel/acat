namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    partial class GraphDisplayerForm2x1
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
            this.graphControlTop = new ZedGraph.ZedGraphControl();
            this.graphControlBottom = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // graphControlTop
            // 
            this.graphControlTop.Location = new System.Drawing.Point(13, 13);
            this.graphControlTop.Name = "graphControlTop";
            this.graphControlTop.ScrollGrace = 0D;
            this.graphControlTop.ScrollMaxX = 0D;
            this.graphControlTop.ScrollMaxY = 0D;
            this.graphControlTop.ScrollMaxY2 = 0D;
            this.graphControlTop.ScrollMinX = 0D;
            this.graphControlTop.ScrollMinY = 0D;
            this.graphControlTop.ScrollMinY2 = 0D;
            this.graphControlTop.Size = new System.Drawing.Size(860, 264);
            this.graphControlTop.TabIndex = 0;
            // 
            // graphControlBottom
            // 
            this.graphControlBottom.Location = new System.Drawing.Point(13, 298);
            this.graphControlBottom.Name = "graphControlBottom";
            this.graphControlBottom.ScrollGrace = 0D;
            this.graphControlBottom.ScrollMaxX = 0D;
            this.graphControlBottom.ScrollMaxY = 0D;
            this.graphControlBottom.ScrollMaxY2 = 0D;
            this.graphControlBottom.ScrollMinX = 0D;
            this.graphControlBottom.ScrollMinY = 0D;
            this.graphControlBottom.ScrollMinY2 = 0D;
            this.graphControlBottom.Size = new System.Drawing.Size(860, 264);
            this.graphControlBottom.TabIndex = 1;
            // 
            // GraphDisplayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 574);
            this.Controls.Add(this.graphControlBottom);
            this.Controls.Add(this.graphControlTop);
            this.Name = "GraphDisplayerForm";
            this.Text = "GraphDisplayerForm";
            this.ResumeLayout(false);

        }

        #endregion

        public ZedGraph.ZedGraphControl graphControlTop;
        public ZedGraph.ZedGraphControl graphControlBottom;


    }
}