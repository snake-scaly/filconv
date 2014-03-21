using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FilConvWpf.Encode;
using FilConvWpf.I18n;
using FilConvWpf.Native;
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

                fileSaveAsMenuItem.IsEnabled = true;
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
            ofd.Filter = string.Join("|", GetFileFilterList());
            bool? r = ofd.ShowDialog();
            if (r == null || !r.Value)
            {
                return;
            }

            Open(ofd.FileName);
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var eip = (EncodingImagePresenter)right.ImagePresenter;

            var sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(fileName);
            sfd.Filter = string.Join("|", eip.SaveDelegates.Select(sd => string.Format(
                "{0} ({1})|{1}",
                L10n.GetObject(sd.FormatNameL10nKey),
                string.Join(";", sd.FileNameMasks))));
            sfd.FilterIndex = 1;

            bool? result = sfd.ShowDialog();

            if (result != null && result.Value)
            {
                try
                {
                    eip.SaveDelegates.ElementAt(sfd.FilterIndex - 1).SaveAs(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, (string)L10n.GetObject("ErrorMessageBoxTitle"));
                }
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

        IEnumerable<string> GetFileFilterList()
        {
            IEnumerable<SupportedFile> files = _supportedFiles.AsEnumerable();

            var types = files.Select(f => Tuple.Create(f.Name, string.Join(";", f.Extensions)));

            string allExts = string.Join(";", types.Select(t => t.Item2));
            types = Enumerable
                .Repeat(Tuple.Create("FileFormatNameAllSupported", allExts), 1)
                .Concat(types)
                .Concat(Enumerable.Repeat(Tuple.Create("FileFormatNameAll", "*.*"), 1));

            return types.Select(t => string.Format("{0} ({1})|{1}", L10n.GetObject(t.Item1), t.Item2));
        }

        struct SupportedFile
        {
            public string Name;
            public string[] Extensions;

            public SupportedFile(string name, string[] extensions)
                : this()
            {
                Name = name;
                Extensions = extensions;
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

        static readonly SupportedFile[] _supportedFiles =
        {
            new SupportedFile("FileFormatNameFil", new string[] { "*.fil" }),
            new SupportedFile("FileFormatNameScr", new string[] { "*.scr" }),
            new SupportedFile("FileFormatNameBmp", new string[] { "*.bmp" }),
            new SupportedFile("FileFormatNameJpeg", new string[] { "*.jpg", "*,jpeg" }),
            new SupportedFile("FileFormatNamePng", new string[] { "*.png" }),
            new SupportedFile("FileFormatNameGif", new string[] { "*.gif" }),
            new SupportedFile("FileFormatNameTiff", new string[] { "*.tif", "*.tiff" }),
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
