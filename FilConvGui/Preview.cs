using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using FilLib;
using ImageLib;

namespace FilConvGui
{
    public partial class Preview : UserControl
    {
        PreviewModel model;
        bool updating;

        public event EventHandler<EventArgs> DisplayPictureChange;

        public Preview()
        {
            SuspendLayout();

            InitializeComponent();

            MinimizeComboBox(zoomComboBox);

            model = new PreviewModel();
            model.DisplayPictureChange += model_DisplayPictureChange;

            Update();

            ResumeLayout();
        }

        public string Title
        {
            get { return model.Title; }
            set
            {
                model.Title = value;
                Update();
            }
        }

        public Fil FilPicture
        {
            get { return model.FilPicture; }
            set { model.FilPicture = value; }
        }

        public Bitmap BitmapPicture
        {
            get { return model.BitmapPicture; }
            set { model.BitmapPicture = value; }
        }

        public AgatImageFormat Format
        {
            get { return model.Format; }
        }

        public Bitmap DisplayPicture
        {
            get { return model.DisplayPicture; }
        }

        public bool Encode
        {
            get { return model.Encode; }
            set
            {
                model.Encode = value;
                Update();
            }
        }

        protected void OnConvertedBitmapChange()
        {
            if (DisplayPictureChange != null)
            {
                DisplayPictureChange(this, EventArgs.Empty);
            }
        }

        private void model_DisplayPictureChange(object sender, EventArgs e)
        {
            Update();
            OnConvertedBitmapChange();
        }

        static void MinimizeComboBox(ToolStripComboBox box)
        {
            int maxWidth = 0;
            foreach (object item in box.Items)
            {
                Size size = TextRenderer.MeasureText(item.ToString(), box.Font);
                maxWidth = Math.Max(size.Width, maxWidth);
            }
            maxWidth += SystemInformation.VerticalScrollBarWidth;
            int oldHeight = box.ComboBox.ClientSize.Height;
            box.ComboBox.ClientSize = new Size(maxWidth, oldHeight);
        }

        /// <summary>
        /// Reflect any changes in the model.
        /// </summary>
        void Update()
        {
            if (updating)
            {
                return;
            }
            updating = true;

            var scale = new Dictionary<PictureScale, int>
            {
                { PictureScale.Single, 0 },
                { PictureScale.Double, 1 },
                { PictureScale.Triple, 2 },
                { PictureScale.Free, 3 },
            };

            titleLabel.Text = model.Title;

            if (model.DisplayFormatBox)
            {
                formatComboBox.BeginUpdate();
                formatComboBox.Items.Clear();
                formatComboBox.Items.AddRange(model.PreviewModes.ToArray());
                formatComboBox.SelectedIndex = model.CurrentPreviewMode;
                MinimizeComboBox(formatComboBox);
                formatComboBox.EndUpdate();
                formatComboBox.Visible = true;
            }
            else
            {
                formatComboBox.Visible = false;
            }

            zoomComboBox.SelectedIndex = scale[model.Scale];
            aspectToolStripButton.Checked = model.TvAspect;
            aspectToolStripButton.Enabled = model.TvAspectEnabled;

            previewPictureBox.SuspendLayout();
            previewPictureBox.Image = model.DisplayPicture;

            if (previewPictureBox.Image != null)
            {
                if (model.Scale.ResizeToFit)
                {
                    previewPictureBox.Dock = DockStyle.Fill;
                    previewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    previewPictureBox.Dock = DockStyle.None;
                    previewPictureBox.Anchor = AnchorStyles.None;
                    previewPictureBox.Width = (int)(previewPictureBox.Image.Width * model.Scale.Scale * model.Aspect);
                    previewPictureBox.Height = (int)(previewPictureBox.Image.Height * model.Scale.Scale);
                    previewPictureBox.Left = (previewPictureBox.Parent.ClientSize.Width - previewPictureBox.Width) / 2;
                    previewPictureBox.Top = (previewPictureBox.Parent.ClientSize.Height - previewPictureBox.Height) / 2;
                    previewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }

            previewPictureBox.ResumeLayout();

            updating = false;
        }

        void formatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, formatComboBox));
            model.CurrentPreviewMode = formatComboBox.SelectedIndex;
        }

        void zoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, zoomComboBox));

            var scale = new PictureScale[] {
                PictureScale.Single,
                PictureScale.Double,
                PictureScale.Triple,
                PictureScale.Free,
            };

            model.Scale = scale[zoomComboBox.SelectedIndex];
            Update();
        }

        private void aspectToolStripButton_CheckStateChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, aspectToolStripButton));
            model.TvAspect = aspectToolStripButton.Checked;
            Update();
        }
    }
}
