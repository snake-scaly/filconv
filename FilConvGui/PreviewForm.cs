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
using FilLib;
using System.IO;
using System.Drawing.Imaging;

namespace FilConvGui
{
    public partial class PreviewForm : Form
    {
        Preview left;
        Preview right;
        string fileName;

        public PreviewForm()
        {
            InitializeComponent();

            left = new Preview();
            left.Dock = DockStyle.Fill;
            left.Title = "Оригинал";
            left.DisplayPictureChange += left_ConvertedBitmapChange;
            splitContainer1.Panel1.Controls.Add(left);

            right = new Preview();
            right.Dock = DockStyle.Fill;
            right.Title = "Результат";
            right.Encode = true;
            splitContainer1.Panel2.Controls.Add(right);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = string.Join("|", GetFileFilterList(true, true).Select(ff => ff.Filter));
            DialogResult r = ofd.ShowDialog();
            if (r != DialogResult.OK)
            {
                return;
            }

            Open(ofd.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void left_ConvertedBitmapChange(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, left));
            right.BitmapPicture = left.DisplayPicture;
        }

        void Open(string fileName)
        {
            try
            {
                if (fileName.EndsWith(".fil", StringComparison.InvariantCultureIgnoreCase))
                {
                    left.FilPicture = Fil.FromFile(fileName);
                }
                else
                {
                    left.BitmapPicture = (Bitmap)Image.FromFile(fileName);
                }
                this.fileName = fileName;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    string.Format("Не удалось загрузить изображение [{0}]: {1}", fileName, e.Message),
                    "Fil Converter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        void Save(string fileName, ImageFormat format)
        {
            if (format != null)
            {
                right.DisplayPicture.Save(fileName, format);
            }
            else
            {
                var fil = new Fil(Path.GetFileName(fileName));
                fil.Data = AgatImageConverter.GetBytes(left.DisplayPicture, right.Format);
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    fil.Write(fs);
                }
            }
        }

        private void PreviewForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PreviewForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object[] files = (object[])e.Data.GetData(DataFormats.FileDrop);
                Open(files[0].ToString());
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IEnumerable<FileFilter> filters = GetFileFilterList(right.Format != null, false);

            var sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(fileName);
            sfd.Filter = string.Join("|", filters.Select(ff => ff.Filter));
            sfd.FilterIndex = 1;
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                fileName = sfd.FileName;
                ImageFormat format = filters.Skip(sfd.FilterIndex - 1).First().ImageFormat;
                Save(fileName, format);
            }
        }

        IEnumerable<FileFilter> GetFileFilterList(bool includeAgat, bool includeGeneric)
        {
            IEnumerable<SupportedFile> files = _supportedPcFiles.AsEnumerable();
            if (includeAgat)
            {
                files = _supportedAgatFiles.Concat(files);
            }

            var types = files.Select(f => Tuple.Create(
                f.Name,
                string.Join(";", f.Extensions),
                f.ImageFormat));

            if (includeGeneric)
            {
                string allExts = string.Join(";", types.Select(t => t.Item2));
                types = Enumerable
                    .Repeat(Tuple.Create("Все поддерживаемые", allExts, (ImageFormat)null), 1)
                    .Concat(types)
                    .Concat(Enumerable.Repeat(Tuple.Create("Все", "*.*", (ImageFormat)null), 1));
            }

            return types.Select(t => new FileFilter(
                string.Format("{0} ({1})|{1}", t.Item1, t.Item2),
                t.Item3));
        }

        struct SupportedFile
        {
            public string Name;
            public string[] Extensions;
            public ImageFormat ImageFormat; // null means FIL

            public SupportedFile(string name, string[] extensions, ImageFormat imageFormat)
                : this()
            {
                Name = name;
                Extensions = extensions;
                ImageFormat = imageFormat;
            }
        }

        struct FileFilter
        {
            public string Filter;
            public ImageFormat ImageFormat;

            public FileFilter(string filter, ImageFormat imageFormat)
            {
                Filter = filter;
                ImageFormat = imageFormat;
            }
        }

        static readonly SupportedFile[] _supportedAgatFiles =
        {
            new SupportedFile("Fil", new string[] { "*.fil" }, null),
        };

        static readonly SupportedFile[] _supportedPcFiles =
        {
            new SupportedFile("Bmp", new string[] { "*.bmp" }, ImageFormat.Bmp),
            new SupportedFile("Jpeg", new string[] { "*.jpg", "*,jpeg" }, ImageFormat.Jpeg),
            new SupportedFile("Png", new string[] { "*.png" }, ImageFormat.Png),
            new SupportedFile("Gif", new string[] { "*.gif" }, ImageFormat.Gif),
            new SupportedFile("Tiff", new string[] { "*.tif", "*.tiff" }, ImageFormat.Tiff),
        };

        private void опрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }
    }
}
