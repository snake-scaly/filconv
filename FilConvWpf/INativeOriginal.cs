using ImageLib;

namespace FilConvWpf
{
    interface INativeOriginal : IOriginal
    {
        NativeImage NativeImage { get; }
        INativeImageFormat NativeImageFormat { get; }
    }
}
