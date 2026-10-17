namespace UsbCamera
{
    partial class MainForm
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
            try
            {
                if (disposing)
                {
                    DisposeCameraResources();
                    components?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBoxPreview = new PictureBox();
            comboBoxCameras = new ComboBox();
            buttonRefresh = new Button();
            buttonConnect = new Button();
            buttonPhoto = new Button();
            buttonRecord = new Button();
            checkBoxMirror = new CheckBox();
            trackBarContrast = new TrackBar();
            labelContrast = new Label();
            labelBrightness = new Label();
            trackBarBrightness = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarContrast).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.BackColor = Color.Black;
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(20, 20);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(720, 509);
            pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPreview.TabIndex = 0;
            pictureBoxPreview.TabStop = false;
            // 
            // comboBoxCameras
            // 
            comboBoxCameras.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCameras.FormattingEnabled = true;
            comboBoxCameras.Location = new Point(770, 50);
            comboBoxCameras.Name = "comboBoxCameras";
            comboBoxCameras.Size = new Size(280, 23);
            comboBoxCameras.TabIndex = 1;
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new Point(770, 100);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(280, 35);
            buttonRefresh.TabIndex = 2;
            buttonRefresh.Text = "Refresh Cameras";
            buttonRefresh.UseVisualStyleBackColor = true;
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(770, 150);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(280, 35);
            buttonConnect.TabIndex = 3;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            // 
            // buttonPhoto
            // 
            buttonPhoto.Location = new Point(840, 215);
            buttonPhoto.Name = "buttonPhoto";
            buttonPhoto.Size = new Size(135, 40);
            buttonPhoto.TabIndex = 4;
            buttonPhoto.Text = "Take Photo";
            buttonPhoto.UseVisualStyleBackColor = true;
            // 
            // buttonRecord
            // 
            buttonRecord.Location = new Point(840, 270);
            buttonRecord.Name = "buttonRecord";
            buttonRecord.Size = new Size(135, 40);
            buttonRecord.TabIndex = 5;
            buttonRecord.Text = "Start Recording";
            buttonRecord.UseVisualStyleBackColor = true;
            // 
            // checkBoxMirror
            // 
            checkBoxMirror.Location = new Point(968, 372);
            checkBoxMirror.Name = "checkBoxMirror";
            checkBoxMirror.Size = new Size(104, 24);
            checkBoxMirror.TabIndex = 12;
            checkBoxMirror.Text = "Mirror image";
            // 
            // trackBarContrast
            // 
            trackBarContrast.Location = new Point(756, 453);
            trackBarContrast.Name = "trackBarContrast";
            trackBarContrast.Size = new Size(200, 45);
            trackBarContrast.TabIndex = 10;
            // 
            // labelContrast
            // 
            labelContrast.AutoSize = true;
            labelContrast.Location = new Point(816, 428);
            labelContrast.Name = "labelContrast";
            labelContrast.Size = new Size(64, 15);
            labelContrast.TabIndex = 11;
            labelContrast.Text = "Contrast: 1.0";
            // 
            // labelBrightness
            // 
            labelBrightness.AutoSize = true;
            labelBrightness.Location = new Point(816, 347);
            labelBrightness.Name = "labelBrightness";
            labelBrightness.Size = new Size(74, 15);
            labelBrightness.TabIndex = 15;
            labelBrightness.Text = "Brightness: 0";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Location = new Point(756, 372);
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(200, 45);
            trackBarBrightness.TabIndex = 14;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1084, 546);
            Controls.Add(labelBrightness);
            Controls.Add(trackBarBrightness);
            Controls.Add(labelContrast);
            Controls.Add(trackBarContrast);
            Controls.Add(checkBoxMirror);
            Controls.Add(buttonRecord);
            Controls.Add(buttonPhoto);
            Controls.Add(buttonConnect);
            Controls.Add(buttonRefresh);
            Controls.Add(comboBoxCameras);
            Controls.Add(pictureBoxPreview);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "USB Camera";
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarContrast).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxPreview;
        private ComboBox comboBoxCameras;
        private Button buttonRefresh;
        private Button buttonConnect;
        private Button buttonPhoto;
        private Button buttonRecord;
        private CheckBox checkBoxMirror;
        private TrackBar trackBarContrast;
        private Label labelContrast;
        private Label labelBrightness;
        private TrackBar trackBarBrightness;
    }
}
