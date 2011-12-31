namespace FilConvGui
{
    partial class Preview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.titleLabel = new System.Windows.Forms.ToolStripLabel();
            this.formatComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.zoomComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.previewContainerPanel = new System.Windows.Forms.Panel();
            this.previewPictureBox = new System.Windows.Forms.PictureBox();
            this.aspectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ditherToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.previewContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleLabel,
            this.formatComboBox,
            this.zoomComboBox,
            this.aspectToolStripButton,
            this.ditherToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(450, 28);
            this.toolStrip.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0, 1, 6, 2);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(78, 25);
            this.titleLabel.Text = "Название";
            // 
            // formatComboBox
            // 
            this.formatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.Size = new System.Drawing.Size(121, 28);
            this.formatComboBox.SelectedIndexChanged += new System.EventHandler(this.formatComboBox_SelectedIndexChanged);
            // 
            // zoomComboBox
            // 
            this.zoomComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.zoomComboBox.IntegralHeight = false;
            this.zoomComboBox.Items.AddRange(new object[] {
            "100%",
            "200%",
            "300%",
            "По окну"});
            this.zoomComboBox.Name = "zoomComboBox";
            this.zoomComboBox.Size = new System.Drawing.Size(121, 28);
            this.zoomComboBox.SelectedIndexChanged += new System.EventHandler(this.zoomComboBox_SelectedIndexChanged);
            // 
            // previewContainerPanel
            // 
            this.previewContainerPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.previewContainerPanel.Controls.Add(this.previewPictureBox);
            this.previewContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewContainerPanel.Location = new System.Drawing.Point(0, 28);
            this.previewContainerPanel.Name = "previewContainerPanel";
            this.previewContainerPanel.Size = new System.Drawing.Size(450, 276);
            this.previewContainerPanel.TabIndex = 1;
            // 
            // previewPictureBox
            // 
            this.previewPictureBox.Location = new System.Drawing.Point(151, 109);
            this.previewPictureBox.Name = "previewPictureBox";
            this.previewPictureBox.Size = new System.Drawing.Size(100, 50);
            this.previewPictureBox.TabIndex = 0;
            this.previewPictureBox.TabStop = false;
            // 
            // aspectToolStripButton
            // 
            this.aspectToolStripButton.CheckOnClick = true;
            this.aspectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.aspectToolStripButton.Image = global::FilConvGui.Properties.Resources.television;
            this.aspectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aspectToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 1, 2);
            this.aspectToolStripButton.Name = "aspectToolStripButton";
            this.aspectToolStripButton.Size = new System.Drawing.Size(23, 25);
            this.aspectToolStripButton.Text = "Пропорции Агата";
            this.aspectToolStripButton.CheckStateChanged += new System.EventHandler(this.aspectToolStripButton_CheckStateChanged);
            // 
            // ditherToolStripButton
            // 
            this.ditherToolStripButton.CheckOnClick = true;
            this.ditherToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ditherToolStripButton.Image = global::FilConvGui.Properties.Resources.rainbow;
            this.ditherToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ditherToolStripButton.Name = "ditherToolStripButton";
            this.ditherToolStripButton.Size = new System.Drawing.Size(23, 25);
            this.ditherToolStripButton.Text = "Улучшенное преобразование цветов";
            this.ditherToolStripButton.CheckStateChanged += new System.EventHandler(this.ditherToolStripButton_CheckStateChanged);
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.previewContainerPanel);
            this.Controls.Add(this.toolStrip);
            this.Name = "Preview";
            this.Size = new System.Drawing.Size(450, 304);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.previewContainerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripComboBox formatComboBox;
        private System.Windows.Forms.ToolStripComboBox zoomComboBox;
        private System.Windows.Forms.Panel previewContainerPanel;
        private System.Windows.Forms.PictureBox previewPictureBox;
        private System.Windows.Forms.ToolStripLabel titleLabel;
        private System.Windows.Forms.ToolStripButton aspectToolStripButton;
        private System.Windows.Forms.ToolStripButton ditherToolStripButton;
    }
}
