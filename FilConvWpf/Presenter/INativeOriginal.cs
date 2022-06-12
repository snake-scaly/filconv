using ImageLib;

namespace FilConvWpf.Presenter
{
    interface INativeOriginal : IOriginal
    {
        NativeImage NativeImage { get; }
        INativeImageFormat NativeImageFormat { get; }
    }
}
