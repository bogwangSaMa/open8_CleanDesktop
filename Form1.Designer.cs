using System.Reflection.Emit;

namespace VolumeControl
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TrackBar trackBarVolume;
        private Label labelVolume;
        private TrackBar trackBarMicVolume;
        private Label labelMicVolume;
        private TrackBar[] trackBars;
        private Label[] labels;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.labelVolume = new System.Windows.Forms.Label();
            this.trackBarMicVolume = new System.Windows.Forms.TrackBar();
            this.labelMicVolume = new System.Windows.Forms.Label();
            this.trackBars = new System.Windows.Forms.TrackBar[10];
            this.labels = new System.Windows.Forms.Label[10];
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMicVolume)).BeginInit();
            this.SuspendLayout();

            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Location = new System.Drawing.Point(12, 12);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(260, 45);
            this.trackBarVolume.TabIndex = 0;

            // 
            // labelVolume
            // 
            this.labelVolume.AutoSize = true;
            this.labelVolume.Location = new System.Drawing.Point(12, 60);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(49, 13);
            this.labelVolume.TabIndex = 1;
            this.labelVolume.Text = "Volume: ";

            // 
            // trackBarMicVolume
            // 
            this.trackBarMicVolume.Location = new System.Drawing.Point(12, 90);
            this.trackBarMicVolume.Name = "trackBarMicVolume";
            this.trackBarMicVolume.Size = new System.Drawing.Size(260, 45);
            this.trackBarMicVolume.TabIndex = 2;

            // 
            // labelMicVolume
            // 
            this.labelMicVolume.AutoSize = true;
            this.labelMicVolume.Location = new System.Drawing.Point(12, 140);
            this.labelMicVolume.Name = "labelMicVolume";
            this.labelMicVolume.Size = new System.Drawing.Size(66, 13);
            this.labelMicVolume.TabIndex = 3;
            this.labelMicVolume.Text = "Mic Volume: ";

            // 이퀄라이저 트랙바 및 레이블 초기화
            string[] freqLabels = { "32Hz", "64Hz", "125Hz", "250Hz", "500Hz", "1kHz", "2kHz", "4kHz", "8kHz", "16kHz" };
            for (int i = 0; i < 10; i++)
            {
                this.trackBars[i] = new TrackBar();
                this.labels[i] = new Label();

                this.trackBars[i].Location = new System.Drawing.Point(12 + i * 30, 170);
                this.trackBars[i].Name = "trackBar" + i;
                this.trackBars[i].Orientation = Orientation.Vertical;
                this.trackBars[i].Size = new System.Drawing.Size(45, 150);
                this.trackBars[i].TabIndex = 4 + i;
                this.trackBars[i].Minimum = -10;
                this.trackBars[i].Maximum = 10;
                this.trackBars[i].Value = 0;

                this.labels[i].Location = new System.Drawing.Point(12 + i * 30, 320);
                this.labels[i].Name = "label" + i;
                this.labels[i].Size = new System.Drawing.Size(45, 13);
                this.labels[i].TabIndex = 14 + i;
                this.labels[i].Text = freqLabels[i];

                this.Controls.Add(this.trackBars[i]);
                this.Controls.Add(this.labels[i]);
            }

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.labelMicVolume);
            this.Controls.Add(this.trackBarMicVolume);
            this.Controls.Add(this.labelVolume);
            this.Controls.Add(this.trackBarVolume);
            this.Name = "Form1";
            this.Text = "Volume Control with Equalizer";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMicVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
