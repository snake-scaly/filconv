using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilConvWpf.Presenter;

namespace FilConvWpf
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : UserControl
    {
        private readonly PreviewModel _model;
        private Point _dragOrigin;
        private Point _dragInitialOffset;

        public Preview()
        {
            InitializeComponent();
            _model = new PreviewModel();
            ScaleComboBox.ScaleChanged += (s, e) => _model.Scale = ScaleComboBox.Scale;
            InnerRoot.DataContext = _model;
        }

        public string Title
        {
            get => _model.Title;
            set => _model.Title = value;
        }

        internal IImagePresenter ImagePresenter
        {
            get => _model.ImagePresenter;
            set => _model.ImagePresenter = value;
        }

        private void BitmapView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _dragOrigin = e.GetPosition(ScrollViewer);
            _dragInitialOffset = new Point(ScrollViewer.ContentHorizontalOffset, ScrollViewer.ContentVerticalOffset);
            BitmapView.CaptureMouse();
        }

        private void BitmapView_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            BitmapView.ReleaseMouseCapture();
        }

        private void BitmapView_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var dragPosition = e.GetPosition(ScrollViewer);
                ScrollViewer.ScrollToHorizontalOffset(_dragInitialOffset.X + _dragOrigin.X - dragPosition.X);
                ScrollViewer.ScrollToVerticalOffset(_dragInitialOffset.Y + _dragOrigin.Y - dragPosition.Y);
            }
        }
    }
}
