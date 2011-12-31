namespace FilConvGui
{
    partial class AboutBox
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.siteLink = new System.Windows.Forms.LinkLabel();
            this.iconsLink = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(161, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 28);
            this.button1.TabIndex = 0;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(154, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Fil Converter";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(218, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "(c) 2011 Сергей \"SnakE\" Громов";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 87);
            this.label3.Margin = new System.Windows.Forms.Padding(4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Разработано для";
            // 
            // siteLink
            // 
            this.siteLink.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.siteLink.AutoSize = true;
            this.siteLink.Location = new System.Drawing.Point(116, 112);
            this.siteLink.Margin = new System.Windows.Forms.Padding(4);
            this.siteLink.Name = "siteLink";
            this.siteLink.Size = new System.Drawing.Size(178, 17);
            this.siteLink.TabIndex = 4;
            this.siteLink.TabStop = true;
            this.siteLink.Text = "http://deka.ssmu.ru/er/agat";
            this.siteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            // 
            // iconsLink
            // 
            this.iconsLink.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.iconsLink.AutoSize = true;
            this.iconsLink.Location = new System.Drawing.Point(74, 174);
            this.iconsLink.Margin = new System.Windows.Forms.Padding(4);
            this.iconsLink.Name = "iconsLink";
            this.iconsLink.Size = new System.Drawing.Size(262, 17);
            this.iconsLink.TabIndex = 5;
            this.iconsLink.TabStop = true;
            this.iconsLink.Text = "http://www.famfamfam.com/lab/icons/silk/";
            this.iconsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 149);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 16, 4, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(358, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "В интерфейсе используются иконки Марка Джеймса";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 262);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.iconsLink);
            this.Controls.Add(this.siteLink);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AboutBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "О программе";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel siteLink;
        private System.Windows.Forms.LinkLabel iconsLink;
        private System.Windows.Forms.Label label4;
    }
}