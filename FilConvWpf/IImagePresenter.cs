namespace FilConvWpf
{
    interface IImagePresenter : IDisplayImageProvider, IToolbarClient
    {
        bool EnableAspectCorrection { get; }
    }
}
