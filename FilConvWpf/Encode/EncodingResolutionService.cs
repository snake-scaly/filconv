using ImageLib;
using ImageLib.Agat;
using ImageLib.Apple;
using ImageLib.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilConvWpf.Encode
{
    /// <summary>
    /// Builds a list of encodings for a particular source image.
    /// </summary>
    static class EncodingResolutionService
    {
        private const int UncompressedBolStartAddress = 0x4000;

        public static IEnumerable<IEncoding> GetPossibleEncodings(IOriginal original)
        {
            foreach (IEncoding e in _genericEncodings)
            {
                yield return e;
            }

            if (original is INativeOriginal)
            {
                var nativeOriginal = (INativeOriginal)original;

                if (nativeOriginal.NativeImageFormat is SpectrumImageFormatInterleave)
                {
                    NativeImage piclerImage = new SpectrumImageFormatInterleave().Deinterleave(nativeOriginal.NativeImage);
                    var saveDelegate = new FilSaveDelegate(piclerImage.Data, ".bol");
                    saveDelegate.StartAddress = UncompressedBolStartAddress;
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
            new NativeEncoding("FormatNameGR7", new Gr7ImageFormat(), true),
            new NativeEncoding("FormatNameMGR", new MgrImageFormat(), true),
            new NativeEncoding("FormatNameHGR", new HgrImageFormat(), true),
            new NativeEncoding("FormatNameMGR9", new Mgr9ImageFormat(), true),
            new NativeEncoding("FormatNameHGR9", new Hgr9ImageFormat(), true),
            new NativeEncoding("FormatNameDMGR", new DmgrImageFormat(), true),
            new NativeEncoding("FormatNameApple2LoRes", new Apple2LoResImageFormat(false), false),
            new NativeEncoding("FormatNameApple2DoubleLoRes", new Apple2LoResImageFormat(true), false),
            new AppleHiResEncoding(),
            new NativeEncoding("FormatNameApple2DoubleHiRes", new Apple2DoubleHiResImageFormat(), false),
        };
    }
}
