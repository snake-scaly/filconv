using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ImageLib;

namespace FilConvGui
{
    public partial class PreviewForm : Form
    {
        PreviewFormController _controller;

        public PreviewForm()
        {
            InitializeComponent();
            _controller = new PreviewFormController(this);
            _controller.Initialize();
        }

        internal void Update(PreviewFormModel model)
        {
            var scale = new Tuple<PictureScale, ToolStripMenuItem>[]
            {
                new Tuple<PictureScale, ToolStripMenuItem>(PictureScale.Single, scale100MenuItem),
                new Tuple<PictureScale, ToolStripMenuItem>(PictureScale.Double, scale200MenuItem),
                new Tuple<PictureScale, ToolStripMenuItem>(PictureScale.Triple, scale300MenuItem),
                new Tuple<PictureScale, ToolStripMenuItem>(PictureScale.Free, scaleFreeMenuItem),
            };

            var aspect = new Tuple<PictureAspect, ToolStripMenuItem>[]
            {
                new Tuple<PictureAspect, ToolStripMenuItem>(PictureAspect.Original, aspectOriginalMenuItem),
                new Tuple<PictureAspect, ToolStripMenuItem>(PictureAspect.Television, aspectTeleMenuItem),
            };

            foreach (var t in scale)
            {
                t.Item2.Checked = t.Item1 == model.Scale;
            }

            foreach (var t in aspect)
            {
                t.Item2.Checked = t.Item1 == model.Aspect;
            }

            if (model.BitmapPicture != null)
            {
                previewPictureBox.Image = model.BitmapPicture;
            }
            else if (model.FilPicture != null)
            {
                Debug.Assert(model.FilPictureFormat != null);
                previewPictureBox.Image = AgatImageConverter.GetBitmap(
                    model.FilPicture.Data,
                    model.FilPictureFormat);
            }

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
                    previewPictureBox.Width = (int)(previewPictureBox.Image.Width * model.Scale.Scale * model.Aspect.Aspect);
                    previewPictureBox.Height = (int)(previewPictureBox.Image.Height * model.Scale.Scale);
                    previewPictureBox.Left = (previewContainer.ClientSize.Width - previewPictureBox.Width) / 2;
                    previewPictureBox.Top = (previewContainer.ClientSize.Height - previewPictureBox.Height) / 2;
                    previewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.Open();
        }

        private void scale100MenuItem_Click(object sender, EventArgs e)
        {
            _controller.SetScale(PictureScale.Single);
        }

        private void scale200MenuItem_Click(object sender, EventArgs e)
        {
            _controller.SetScale(PictureScale.Double);
        }

        private void scale300MenuItem_Click(object sender, EventArgs e)
        {
            _controller.SetScale(PictureScale.Triple);
        }

        private void scaleFreeMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SetScale(PictureScale.Free);
        }

        private void aspectOriginalMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SetAspect(PictureAspect.Original);
        }

        private void aspectTeleMenuItem_Click(object sender, EventArgs e)
        {
            _controller.SetAspect(PictureAspect.Television);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.Exit();
        }

        private void previewContainer_DragDrop(object sender, DragEventArgs e)
        {
            _controller.DragDrop(e);
        }

        private void previewContainer_DragEnter(object sender, DragEventArgs e)
        {
            _controller.DragEnter(e);
        }
    }
}
