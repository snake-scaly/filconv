using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FilLib;
using ImageLib;
using ImageLib.Agat;
using NDesk.Options;
using SkiaSharp;

namespace filconv
{
    class Program
    {
        private const string filExtension = ".fil";

        static int Main(string[] args)
        {
            string filFormatName = "CGSR";
            string filTypeName = "B";
            bool dither = false;

            string agatFormats = string.Join(", ", _agatFormats.Select(f => f.Item2));

            var options = new OptionSet()
            {
                {
                    "g|gfx-format=",
                    $"Agat image format (case-insensitive): {agatFormats}.  Default is {filFormatName}",
                    v => {
                        if (GetFormatByName(v) == null)
                            throw new OptionException($"Must be one of {agatFormats}", "gfx-format");
                        filFormatName = v;
                    }
                },
                {
                    "t|type=",
                    "type of Agat file to create.  One of T, B or K (case-insensitive).  " +
                        "Default is B.  Only used when converting an image into Agat format",
                    v => {
                        v = v.ToUpper();
                        if (v.Length != 1 || "TBK".IndexOf(v) == -1)
                            throw new OptionException("Must be one of T, B or K", "type");
                        filTypeName = v;
                    }
                },
                {
                    "d|dither",
                    "enable color dithering when reducing number of colors",
                    v => {
                        dither = true;
                    }
                },
            };

            List<string> positional;
            try
            {
                positional = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Usage(options);
                return 1;
            }

            if (positional.Count != 2)
            {
                Usage(options);
                return 1;
            }

            INativeImageFormat filFormat = GetFormatByName(filFormatName);

            string src = positional[0];
            string dst = positional[1];

            if (src.ToLower().EndsWith(filExtension))
            {
                // converting from Agat to PC
                string ext = Path.GetExtension(dst);
                if (!_extToFormat.TryGetValue(ext, out var imgFormat))
                {
                    Console.Error.WriteLine("Supported PC formats are {0} but output file is {1}", string.Join(", ", _extToFormat.Keys), dst);
                    return 1;
                }
                FromFil(src, dst, filFormat, imgFormat);
            }
            else if (dst.ToLower().EndsWith(filExtension))
            {
                // converting from PC to Agat
                ToFil(src, dst, filFormat, FilType.FromName(filTypeName[0]), dither);
            }
            else
            {
                Console.Error.WriteLine("Neither of files have a {0} extension", filExtension);
                return 1;
            }

            return 0;
        }

        static void FromFil(string from, string to, INativeImageFormat formatFrom, SKEncodedImageFormat formatTo)
        {
            var fil = Fil.FromFile(from);
            var ni = new NativeImage { Data = fil.GetData(), FormatHint = new FormatHint(Path.GetExtension(from)) };
            var bitmapData = formatFrom.FromNative(ni, formatFrom.GetDefaultDecodingOptions(ni)).Bitmap;

            var bmp = new SKBitmap();
            var pixels = GCHandle.Alloc(bitmapData.Pixels, GCHandleType.Pinned);

            try
            {
                var imageInfo = new SKImageInfo(bitmapData.Width, bitmapData.Height, SKColorType.Bgra8888, SKAlphaType.Opaque);
                bmp.InstallPixels(imageInfo, pixels.AddrOfPinnedObject());
                using var toStream = File.Create(to);
                bmp.Encode(toStream, formatTo, 0);
            }
            finally
            {
                pixels.Free();
            }
        }

        static void ToFil(string from, string to, INativeImageFormat format, FilType type, bool dither)
        {
            var bmp = SKBitmap.Decode(from);
            EncodingOptions options = new EncodingOptions();
            options.Dither = dither;
            byte[] pixels = format.ToNative(new BitmapPixels(bmp), options).Data;
            var fil = new Fil { Name = Path.GetFileNameWithoutExtension(to), Type = type };
            fil.SetData(pixels);
            using (var fs = new FileStream(to, FileMode.Create))
            {
                fil.Write(fs);
            }
        }

        static void Usage(OptionSet options)
        {
            string exe = Process.GetCurrentProcess().ProcessName;
            Console.WriteLine("Usage: {0} [options] <infile> <outfile>", exe);
            Console.WriteLine("The file extensions determine the action and the output format.");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        static INativeImageFormat GetFormatByName(string name)
        {
            foreach (var f in _agatFormats)
            {
                if (String.Compare(f.Item2, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return f.Item1;
                }
            }
            return null;
        }

        static readonly (INativeImageFormat, string)[] _agatFormats =
        {
            (new AgatCGNRImageFormat(), "CGNR"),
            (new AgatCGSRImageFormat(), "CGSR"),
            (new AgatMGVRImageFormat(), "MGVR"),
            (new AgatCGVRImageFormat(), "CGVR"),
            (new AgatMGDPImageFormat(), "MGDP"),
        };

        static readonly Dictionary<string, SKEncodedImageFormat> _extToFormat =
            new(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".png", SKEncodedImageFormat.Png },
            { ".jpg", SKEncodedImageFormat.Jpeg },
            { ".gif", SKEncodedImageFormat.Gif },
            { ".bmp", SKEncodedImageFormat.Bmp },
        };
    }
}
