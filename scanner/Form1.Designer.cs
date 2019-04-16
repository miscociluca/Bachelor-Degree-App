namespace CameraCapture
{
    partial class CameraCapture
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
            this.CamImgBox = new Emgu.CV.UI.ImageBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.CaptureBox = new Emgu.CV.UI.ImageBox();
            this.Capturebtn = new System.Windows.Forms.Button();
            this.Savebtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CamImgBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CaptureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CamImgBox
            // 
            this.CamImgBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CamImgBox.Location = new System.Drawing.Point(52, 15);
            this.CamImgBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CamImgBox.Name = "CamImgBox";
            this.CamImgBox.Size = new System.Drawing.Size(493, 322);
            this.CamImgBox.TabIndex = 2;
            this.CamImgBox.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(249, 364);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 28);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click_1);
            // 
            // CaptureBox
            // 
            this.CaptureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CaptureBox.Location = new System.Drawing.Point(656, 15);
            this.CaptureBox.Margin = new System.Windows.Forms.Padding(4);
            this.CaptureBox.Name = "CaptureBox";
            this.CaptureBox.Size = new System.Drawing.Size(493, 322);
            this.CaptureBox.TabIndex = 4;
            this.CaptureBox.TabStop = false;
            // 
            // Capturebtn
            // 
            this.Capturebtn.Location = new System.Drawing.Point(760, 364);
            this.Capturebtn.Margin = new System.Windows.Forms.Padding(4);
            this.Capturebtn.Name = "Capturebtn";
            this.Capturebtn.Size = new System.Drawing.Size(100, 28);
            this.Capturebtn.TabIndex = 5;
            this.Capturebtn.Text = "Capture";
            this.Capturebtn.UseVisualStyleBackColor = true;
            this.Capturebtn.Click += new System.EventHandler(this.Capturebtn_Click);
            // 
            // Savebtn
            // 
            this.Savebtn.Location = new System.Drawing.Point(957, 364);
            this.Savebtn.Margin = new System.Windows.Forms.Padding(4);
            this.Savebtn.Name = "Savebtn";
            this.Savebtn.Size = new System.Drawing.Size(100, 28);
            this.Savebtn.TabIndex = 6;
            this.Savebtn.Text = "Save";
            this.Savebtn.UseVisualStyleBackColor = true;
            this.Savebtn.Click += new System.EventHandler(this.Savebtn_Click);
            // 
            // CameraCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 464);
            this.Controls.Add(this.Savebtn);
            this.Controls.Add(this.Capturebtn);
            this.Controls.Add(this.CaptureBox);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.CamImgBox);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CameraCapture";
            this.Text = "CameraOutput";
            this.Load += new System.EventHandler(this.CameraCapture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CamImgBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CaptureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox CamImgBox;
        private System.Windows.Forms.Button btnStart;
        private Emgu.CV.UI.ImageBox CaptureBox;
        private System.Windows.Forms.Button Capturebtn;
        private System.Windows.Forms.Button Savebtn;
    }
}

