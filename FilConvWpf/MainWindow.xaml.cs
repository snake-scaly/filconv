using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using FilLib;
using ImageLib;
using Microsoft.Win32;

namespace FilConvWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;

        public MainWindow()
        {
            InitializeComponent();
            left.DisplayPictureChange += left_DisplayPictureChange;
            right.Encode = true;
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
                    left.BitmapPicture = (Bitmap)System.Drawing.Image.FromFile(fileName);
                }
                this.fileName = fileName;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    string.Format("Не удалось загрузить изображение [{0}]: {1}", fileName, e.Message),
                    "Fil Converter",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                var fil = new Fil(System.IO.Path.GetFileName(fileName));
                fil.Data = AgatImageConverter.GetBytes(left.DisplayPicture, right.Format, right.Dither);
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    fil.Write(fs);
                }
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object[] files = (object[])e.Data.GetData(DataFormats.FileDrop);
                Open(files[0].ToString());
            }
        }

        void left_DisplayPictureChange(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, left));
            right.BitmapPicture = left.DisplayPicture;
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = string.Join("|", GetFileFilterList(true, true).Select(ff => ff.Filter));
            bool? r = ofd.ShowDialog();
            if (r == null || !r.Value)
            {
                return;
            }

            Open(ofd.FileName);
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<FileFilter> filters = GetFileFilterList(right.Format != null, false);

            var sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(fileName);
            sfd.Filter = string.Join("|", filters.Select(ff => ff.Filter));
            sfd.FilterIndex = 1;
            bool? result = sfd.ShowDialog();
            if (result != null && result.Value)
            {
                fileName = sfd.FileName;
                ImageFormat format = filters.Skip(sfd.FilterIndex - 1).First().ImageFormat;
                Save(fileName, format);
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Owner = this;
            about.ShowDialog();
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
    }
}
