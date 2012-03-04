using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageLib;

namespace FilConvWpf
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : UserControl
    {
        PreviewModel model;
        bool updating;

        public event EventHandler<EventArgs> DisplayPictureChange;

        public Preview()
        {
            using (new IgnoreEvents())
            {
                InitializeComponent();

                model = new PreviewModel();
                model.DisplayPictureChange += model_DisplayPictureChange;
                model.Toolbar = new ToolbarFragment(toolBar, toolBar.Items.GetItemAt(toolBar.Items.Count - 1), null);

                Update();
            }
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

        internal IImageDisplayAdapter Image
        {
            get { return model.Image; }
            set { model.Image = value; }
        }

        public BitmapSource DisplayPicture
        {
            get { return model.DisplayPicture; }
        }

        protected virtual void OnDisplayPictureChange()
        {
            if (DisplayPictureChange != null)
            {
                DisplayPictureChange(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Reflect any changes in the model.
        /// </summary>
        void Update()
        {
            using (new IgnoreEvents())
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

                titleLabel.Content = model.Title;

                scaleComboBox.SelectedIndex = scale[model.Scale];
                tvAspectToggle.IsChecked = model.TvAspect;
                tvAspectToggle.IsEnabled = model.TvAspectEnabled;

                BitmapSource bs = model.DisplayPicture;
                previewPictureBox.Source = bs;

                if (bs != null)
                {
                    previewPictureBox.Scale = model.Scale.Scale;
                    previewPictureBox.Aspect = model.Aspect;
                }
                previewViewBox.Stretch = model.Scale.ResizeToFit ? Stretch.Uniform : Stretch.None;

                updating = false;
            }
        }

        void model_DisplayPictureChange(object sender, EventArgs e)
        {
            Update();
            OnDisplayPictureChange();
        }

        void zoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IgnoreEvents.Ignore)
            {
                Debug.Assert(object.ReferenceEquals(sender, scaleComboBox));

                var scale = new PictureScale[] {
                    PictureScale.Single,
                    PictureScale.Double,
                    PictureScale.Triple,
                    PictureScale.Free,
                };

                model.Scale = scale[scaleComboBox.SelectedIndex];
                Update();
            }
        }

        void tvAspectToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (!IgnoreEvents.Ignore)
            {
                model.TvAspect = true;
                Update();
            }
        }

        void tvAspectToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IgnoreEvents.Ignore)
            {
                model.TvAspect = false;
                Update();
            }
        }

        sealed class IgnoreEvents : IDisposable
        {
            static int ignore = 0;

            public static bool Ignore { get { return ignore != 0; } }

            public IgnoreEvents()
            {
                ++ignore;
            }

            ~IgnoreEvents()
            {
                Debug.Fail("Object weren't disposed");
            }

            public void Dispose()
            {
                --ignore;
                GC.SuppressFinalize(this);
            }
        }
    }
}
