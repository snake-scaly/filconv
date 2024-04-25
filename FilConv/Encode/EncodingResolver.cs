using System.Collections.Generic;
using FilConv.Presenter;
using ImageLib.Agat;
using ImageLib.Apple;

namespace FilConv.Encode;

/// <summary>
/// Builds a list of encodings for a particular source image.
/// </summary>
public static class EncodingResolver
{
    public static IEnumerable<IEncoding> GetPossibleEncodings(IOriginal original) => _genericEncodings;

    private static readonly IEncoding[] _genericEncodings =
    {
        new IdentityEncoding(),
        new NativeEncoding("FormatNameAgatCGNR", new AgatCGNRImageFormat(), true),
        new NativeEncoding("FormatNameAgatCGSR", new AgatCGSRImageFormat(), true),
        new NativeEncoding("FormatNameAgatMGVR", new AgatMGVRImageFormat(), true),
        new NativeEncoding("FormatNameAgatCGVR", new AgatCGVRImageFormat(), true),
        new NativeEncoding("FormatNameAgatMGDP", new AgatMGDPImageFormat(), true),
        new NativeEncoding("FormatNameAgatApple", new AgatAppleImageFormat(), true),
        new NativeEncoding("FormatNameAgatCGSRDV", new AgatCGSRDVImageFormat(), true),
        new NativeEncoding("FormatNameApple2LoRes", new Apple2LoResImageFormat(false), true),
        new NativeEncoding("FormatNameApple2DoubleLoRes", new Apple2LoResImageFormat(true), true),
        new NativeEncoding("FormatNameApple2HiRes", new Apple2HiResImageFormat(), true),
        new NativeEncoding("FormatNameApple2DoubleHiRes", new Apple2DoubleHiResImageFormat(), true),
    };
}
