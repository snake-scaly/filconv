namespace FilConvGui
{
    partial class PreviewForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scale100MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scale200MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scale300MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleFreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aspectOriginalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aspectTeleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewPictureBox = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.previewContainer = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
            this.previewContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(635, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.fileToolStripMenuItem.Text = "&Файл";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.openToolStripMenuItem.Text = "&Открыть...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.saveAsToolStripMenuItem.Text = "Сохранить &как...";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.exitToolStripMenuItem.Text = "В&ыход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scale100MenuItem,
            this.scale200MenuItem,
            this.scale300MenuItem,
            this.scaleFreeMenuItem,
            this.toolStripSeparator1,
            this.aspectOriginalMenuItem,
            this.aspectTeleMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.viewToolStripMenuItem.Text = "&Вид";
            // 
            // scale100MenuItem
            // 
            this.scale100MenuItem.Name = "scale100MenuItem";
            this.scale100MenuItem.Size = new System.Drawing.Size(199, 24);
            this.scale100MenuItem.Text = "Масштаб 100%";
            this.scale100MenuItem.Click += new System.EventHandler(this.scale100MenuItem_Click);
            // 
            // scale200MenuItem
            // 
            this.scale200MenuItem.Name = "scale200MenuItem";
            this.scale200MenuItem.Size = new System.Drawing.Size(199, 24);
            this.scale200MenuItem.Text = "Масштаб 200%";
            this.scale200MenuItem.Click += new System.EventHandler(this.scale200MenuItem_Click);
            // 
            // scale300MenuItem
            // 
            this.scale300MenuItem.Name = "scale300MenuItem";
            this.scale300MenuItem.Size = new System.Drawing.Size(199, 24);
            this.scale300MenuItem.Text = "Масштаб 300%";
            this.scale300MenuItem.Click += new System.EventHandler(this.scale300MenuItem_Click);
            // 
            // scaleFreeMenuItem
            // 
            this.scaleFreeMenuItem.Name = "scaleFreeMenuItem";
            this.scaleFreeMenuItem.Size = new System.Drawing.Size(199, 24);
            this.scaleFreeMenuItem.Text = "Масштаб по окну";
            this.scaleFreeMenuItem.Click += new System.EventHandler(this.scaleFreeMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // aspectOriginalMenuItem
            // 
            this.aspectOriginalMenuItem.Name = "aspectOriginalMenuItem";
            this.aspectOriginalMenuItem.Size = new System.Drawing.Size(199, 24);
            this.aspectOriginalMenuItem.Text = "Пропорции 1:1";
            this.aspectOriginalMenuItem.Click += new System.EventHandler(this.aspectOriginalMenuItem_Click);
            // 
            // aspectTeleMenuItem
            // 
            this.aspectTeleMenuItem.Name = "aspectTeleMenuItem";
            this.aspectTeleMenuItem.Size = new System.Drawing.Size(199, 24);
            this.aspectTeleMenuItem.Text = "Пропорции 4:3";
            this.aspectTeleMenuItem.Click += new System.EventHandler(this.aspectTeleMenuItem_Click);
            // 
            // previewPictureBox
            // 
            this.previewPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.previewPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.previewPictureBox.Location = new System.Drawing.Point(207, 122);
            this.previewPictureBox.Name = "previewPictureBox";
            this.previewPictureBox.Size = new System.Drawing.Size(220, 159);
            this.previewPictureBox.TabIndex = 1;
            this.previewPictureBox.TabStop = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Fil|*.fil|Все|*.*";
            // 
            // previewContainer
            // 
            this.previewContainer.Controls.Add(this.previewPictureBox);
            this.previewContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewContainer.Location = new System.Drawing.Point(0, 28);
            this.previewContainer.Name = "previewContainer";
            this.previewContainer.Size = new System.Drawing.Size(635, 406);
            this.previewContainer.TabIndex = 2;
            // 
            // PreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 434);
            this.Controls.Add(this.previewContainer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PreviewForm";
            this.Text = "Fil Converter";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
            this.previewContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scale100MenuItem;
        private System.Windows.Forms.ToolStripMenuItem scale200MenuItem;
        private System.Windows.Forms.ToolStripMenuItem scale300MenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaleFreeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aspectOriginalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aspectTeleMenuItem;
        private System.Windows.Forms.PictureBox previewPictureBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel previewContainer;
    }
}

