using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                model.Title = (string) titleLabel.Content;
                model.DisplayPictureChange += model_DisplayPictureChange;

                InnerRoot.DataContext = model;

                Update();
            }
        }

        public string Title
        {
            get => model.Title;

            set
            {
                model.Title = value;
                Update();
            }
        }

        internal IImagePresenter ImagePresenter
        {
            get => model.ImagePresenter;
            set => model.ImagePresenter = value;
        }

        public BitmapSource DisplayPicture => model.DisplayPicture;

        protected virtual void OnDisplayPictureChange()
        {
            DisplayPictureChange?.Invoke(this, EventArgs.Empty);
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

                titleLabel.Content = model.Title;

                var scale = new Dictionary<PictureScale, int>
                {
                    { PictureScale.Single, 0 },
                    { PictureScale.Double, 1 },
                    { PictureScale.Triple, 2 },
                    { PictureScale.Free, 3 },
                };

                scaleComboBox.SelectedIndex = scale[model.Scale];

                nativeAspectToggle.Visibility = model.AspectToggleEnabled ? Visibility.Visible : Visibility.Collapsed;
                nativeAspectToggle.IsChecked = model.AspectToggleChecked;

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
                Debug.Assert(ReferenceEquals(sender, scaleComboBox));

                var scale = new[] {
                    PictureScale.Single,
                    PictureScale.Double,
                    PictureScale.Triple,
                    PictureScale.Free,
                };

                model.Scale = scale[scaleComboBox.SelectedIndex];
                Update();
            }
        }

        void aspectButton_Checked(object sender, RoutedEventArgs e)
        {
            model.AspectToggleChecked = true;
            Update();
        }

        void aspectButton_Unchecked(object sender, RoutedEventArgs e)
        {
            model.AspectToggleChecked = false;
            Update();
        }

        sealed class IgnoreEvents : IDisposable
        {
            static int ignore = 0;

            public static bool Ignore => ignore != 0;

            public IgnoreEvents()
            {
                ++ignore;
            }

            ~IgnoreEvents()
            {
                Debug.Fail("Object weren'target disposed");
            }

            public void Dispose()
            {
                --ignore;
                GC.SuppressFinalize(this);
            }
        }
    }
}
