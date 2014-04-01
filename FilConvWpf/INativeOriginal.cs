using ImageLib;

namespace FilConvWpf
{
    interface INativeOriginal : IOriginal
    {
        NativeImage NativeImage { get; }
        NativeImageFormat NativeImageFormat { get; }
    }
}
