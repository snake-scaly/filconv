using System.Collections.Generic;
using FilConvWpf.Presenter;
using ImageLib;
using ImageLib.Agat;
using ImageLib.Apple;
using ImageLib.Spectrum;

namespace FilConvWpf.Encode
{
    /// <summary>
    /// Builds a list of encodings for a particular source image.
    /// </summary>
    public static class EncodingResolver
    {
        private const int UncompressedBolStartAddress = 0x4000;

        public static IEnumerable<IEncoding> GetPossibleEncodings(IOriginal original)
        {
            foreach (IEncoding e in _genericEncodings)
            {
                yield return e;
            }

            if (original is INativeOriginal nativeOriginal)
            {
                if (nativeOriginal.NativeImageFormat is SpectrumImageFormatInterleave)
                {
                    NativeImage piclerImage = new SpectrumImageFormatInterleave().Deinterleave(nativeOriginal.NativeImage);
                    var saveDelegate = new FilSaveDelegate(piclerImage.Data, ".bol");
                    saveDelegate.LoadAddress = UncompressedBolStartAddress;
                    yield return new Transcoding(piclerImage, new SpectrumImageFormatPicler(), "FormatNamePicler", new ISaveDelegate[] { saveDelegate });
                }
                else if (nativeOriginal.NativeImageFormat is SpectrumImageFormatPicler)
                {
                    var format = new SpectrumImageFormatInterleave();
                    NativeImage spectrumImage = format.Interleave(nativeOriginal.NativeImage);
                    var saveDelegate = new SimpleSaveDelegate(spectrumImage.Data, "FileFormatNameScr", "*.scr");
                    yield return new Transcoding(spectrumImage, format, "FormatNameSpectrum", new ISaveDelegate[] { saveDelegate });
                }
            }
        }

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
}
