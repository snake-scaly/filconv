﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using FilConvWpf.Native;
using FilLib;
using ImageLib;
using Microsoft.Win32;
using FilConvWpf.Encode;
using System.Globalization;
using System.Windows.Controls;
using FilConvWpf.I18n;

namespace FilConvWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;
        private string rawTitle;

        public MainWindow()
        {
            InitializeComponent();
            right.ImagePresenter = new EncodingImagePresenter(left);
            rawTitle = Title;

            foreach (SupportedLanguage l in _supportedLanguages)
            {
                MenuItem item = new MenuItem();
                L10n.AddLocalizedProperty(item, MenuItem.HeaderProperty, l.NameKey).Update();
                item.Tag = l.Culture;
                item.Click += menuLanguage_Click;
                if (l.Culture == L10n.Culture)
                {
                    item.IsChecked = true;
                }
                menuLanguage.Items.Add(item);
            }
        }

        void Open(string fileName)
        {
            try
            {
                if (fileName.EndsWith(".fil", StringComparison.InvariantCultureIgnoreCase))
                {
                    Fil fil = Fil.FromFile(fileName);
                    NativeImage ni = new NativeImage(fil.Data, new FormatHint(".fil"));
                    left.ImagePresenter = new NativeImagePresenter(ni);
                }
                else if (fileName.EndsWith(".scr", StringComparison.InvariantCultureIgnoreCase))
                {
                    NativeImage ni = new NativeImage(File.ReadAllBytes(fileName), new FormatHint(".scr"));
                    left.ImagePresenter = new NativeImagePresenter(ni);
                }
                else
                {
                    left.ImagePresenter = new BitmapPresenter(new BitmapImage(new Uri(fileName)));
                }
                this.fileName = fileName;
                Title = Path.GetFileName(fileName) + " - " + rawTitle;
            }
            catch (NotSupportedException e)
            {
                MessageBox.Show(
                    string.Format((string)L10n.GetObject("UnableToLoadFileError"), fileName, e.Message),
                    "Fil Converter",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        void Save(string fileName, Type encoderType)
        {
            if (encoderType != null)
            {
                BitmapEncoder encoder = (BitmapEncoder)Activator.CreateInstance(encoderType);
                encoder.Frames.Add(BitmapFrame.Create(right.DisplayPicture));
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            else
            {
                var fil = new Fil(Path.GetFileNameWithoutExtension(fileName));
                ((EncodingImagePresenter)right.ImagePresenter).EncodeInto(fil);
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

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = string.Join("|", GetFileFilterList(true, true, true).Select(ff => ff.Filter));
            bool? r = ofd.ShowDialog();
            if (r == null || !r.Value)
            {
                return;
            }

            Open(ofd.FileName);
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var eida = (EncodingImagePresenter)right.ImagePresenter;
            IEnumerable<FileFilter> filters = GetFileFilterList(eida.IsContainerSupported(typeof(Fil)), false, false);

            var sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(fileName);
            sfd.Filter = string.Join("|", filters.Select(ff => ff.Filter));
            sfd.FilterIndex = 1;
            bool? result = sfd.ShowDialog();
            if (result != null && result.Value)
            {
                fileName = sfd.FileName;
                Type format = filters.Skip(sfd.FilterIndex - 1).First().EncoderType;
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

        private void menuLanguage_Click(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in menuLanguage.Items)
            {
                if (Object.ReferenceEquals(item, sender))
                {
                    L10n.Culture = (CultureInfo)item.Tag;
                    item.IsChecked = true;
                }
                else
                {
                    item.IsChecked = false;
                }
            }
        }

        IEnumerable<FileFilter> GetFileFilterList(bool includeAgat, bool includeScr, bool includeGeneric)
        {
            IEnumerable<SupportedFile> files = _supportedPcFiles.AsEnumerable();
            if (includeScr)
            {
                files = _supportedScrFiles.Concat(files);
            }
            if (includeAgat)
            {
                files = _supportedAgatFiles.Concat(files);
            }

            var types = files.Select(f => Tuple.Create(
                f.Name,
                string.Join(";", f.Extensions),
                f.EncoderType));

            if (includeGeneric)
            {
                string allExts = string.Join(";", types.Select(t => t.Item2));
                types = Enumerable
                    .Repeat(Tuple.Create("Все поддерживаемые", allExts, (Type)null), 1)
                    .Concat(types)
                    .Concat(Enumerable.Repeat(Tuple.Create("Все", "*.*", (Type)null), 1));
            }

            return types.Select(t => new FileFilter(
                string.Format("{0} ({1})|{1}", t.Item1, t.Item2),
                t.Item3));
        }

        struct SupportedFile
        {
            public string Name;
            public string[] Extensions;
            public Type EncoderType; // null means FIL

            public SupportedFile(string name, string[] extensions, Type encoderType)
                : this()
            {
                Name = name;
                Extensions = extensions;
                EncoderType = encoderType;
            }
        }

        struct FileFilter
        {
            public string Filter;
            public Type EncoderType;

            public FileFilter(string filter, Type encoderType)
            {
                Filter = filter;
                EncoderType = encoderType;
            }
        }

        struct SupportedLanguage
        {
            public string NameKey;
            public CultureInfo Culture;

            public SupportedLanguage(string nameKey, CultureInfo culture)
            {
                NameKey = nameKey;
                Culture = culture;
            }
        }

        static readonly SupportedFile[] _supportedAgatFiles =
        {
            new SupportedFile("Fil", new string[] { "*.fil" }, null),
        };

        static readonly SupportedFile[] _supportedScrFiles =
        {
            new SupportedFile("Scr", new string[] { "*.scr" }, null),
        };

        static readonly SupportedFile[] _supportedPcFiles =
        {
            new SupportedFile("Bmp", new string[] { "*.bmp" }, typeof(BmpBitmapEncoder)),
            new SupportedFile("Jpeg", new string[] { "*.jpg", "*,jpeg" }, typeof(JpegBitmapEncoder)),
            new SupportedFile("Png", new string[] { "*.png" }, typeof(PngBitmapEncoder)),
            new SupportedFile("Gif", new string[] { "*.gif" }, typeof(GifBitmapEncoder)),
            new SupportedFile("Tiff", new string[] { "*.tif", "*.tiff" }, typeof(TiffBitmapEncoder)),
        };

        static readonly SupportedLanguage[] _supportedLanguages =
        {
            new SupportedLanguage("MenuLanguageAuto", null),
            new SupportedLanguage("MenuLanguageEnglish", new CultureInfo("en")),
            new SupportedLanguage("MenuLanguageRussian", new CultureInfo("ru")),
            new SupportedLanguage("MenuLanguageBulgarian", new CultureInfo("bg")),
        };
    }
}
