using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FilLib;
using ImageLib;
using System.Drawing;

namespace FilConvGui
{
    class PreviewFormController
    {
        PreviewForm _view;
        PreviewFormModel _model;

        public PreviewFormController(PreviewForm view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _model = new PreviewFormModel();
            _model.Scale = PictureScale.Double;
            _model.Aspect = PictureAspect.Television;
            _view.Update(_model);
        }

        public void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = GetFileFilter();
            DialogResult r = ofd.ShowDialog();
            if (r != DialogResult.OK)
            {
                return;
            }

            string fileName = ofd.FileName;

            try
            {
                if (fileName.EndsWith(".fil", StringComparison.InvariantCultureIgnoreCase))
                {
                    _model.FilPicture = Fil.FromFile(fileName);
                    _model.FilPictureFormat = new MgrImageFormat();
                    _model.BitmapPicture = null;
                }
                else
                {
                    _model.BitmapPicture = (Bitmap)Image.FromFile(fileName);
                    _model.FilPicture = null;
                    _model.FilPictureFormat = null;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(
                    string.Format("Не удалось загрузить изображение [{0}]: {1}", fileName, e.Message),
                    "Fil Converter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            _view.Update(_model);
        }

        public void SetScale(PictureScale scale)
        {
            _model.Scale = scale;
            _view.Update(_model);
        }

        public void SetAspect(PictureAspect aspect)
        {
            _model.Aspect = aspect;
            _view.Update(_model);
        }

        public void Exit()
        {
            Application.Exit();
        }

        string GetFileFilter()
        {
            var types = new List<Tuple<string, string>>();
            foreach (SupportedFile sf in _supportedAgatFiles.Concat(_supportedPcFiles))
            {
                string exts = string.Join(";", sf.Extensions);
                types.Add(new Tuple<string, string>(sf.Name, exts));
            }

            string allExts = string.Join(";", types.Select(t => t.Item2));
            types.Insert(0, new Tuple<string, string>("Все поддерживаемые", allExts));
            types.Add(new Tuple<string, string>("Все", "*.*"));

            return string.Join("|", types.Select(t => string.Format("{0} ({1})|{1}", t.Item1, t.Item2)));
        }

        struct SupportedFile
        {
            public String Name;
            public String[] Extensions;

            public SupportedFile(string name, string[] extensions)
                : this()
            {
                Name = name;
                Extensions = extensions;
            }
        }

        static readonly SupportedFile[] _supportedAgatFiles =
        {
            new SupportedFile("Fil", new string[] { "*.fil" })
        };

        static readonly SupportedFile[] _supportedPcFiles =
        {
            new SupportedFile("Bmp", new string[] { "*.bmp" }),
            new SupportedFile("Jpeg", new string[] { "*.jpg", "*,jpeg" }),
            new SupportedFile("Png", new string[] { "*.png" }),
            new SupportedFile("Gif", new string[] { "*.gif" }),
            new SupportedFile("Tiff", new string[] { "*.tif", "*.tiff" }),
        };
    }
}
