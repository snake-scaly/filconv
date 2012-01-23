using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib;
using NDesk.Options;

namespace filconv
{
    class Program
    {
        private const string filExtension = ".fil";

        static int Main(string[] args)
        {
            string filFormatName = "MGR";
            string filTypeName = "B";
            bool dither = false;

            string agatFormats = string.Join(", ", _agatFormats.Select(f => f.Item2));

            var options = new OptionSet()
            {
                {
                    "g|gfx-format=",
                    "Agat image format (case-insensitive): " + agatFormats + ".  Default is MGR",
                    v => {
                        if (GetFormatByName(v) == null)
                            throw new OptionException("Must be either MGR or HGR", "gfx-format");
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
                System.Console.WriteLine(e.Message);
                Usage(options);
                return 1;
            }

            if (positional.Count != 2)
            {
                Usage(options);
                return 1;
            }

            NativeImageFormat filFormat = GetFormatByName(filFormatName);

            string src = positional[0];
            string dst = positional[1];

            if (src.ToLower().EndsWith(filExtension))
            {
                // converting from Agat to PC
                string ext = Path.GetExtension(dst);
                Type imgFormat;
                if (!_extToFormat.TryGetValue(ext, out imgFormat))
                {
                    System.Console.Error.WriteLine("Supported PC formats are {0} but output file is {1}", string.Join(", ", _extToFormat.Keys), dst);
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
                System.Console.Error.WriteLine("Neither of files have a {0} extension", filExtension);
                return 1;
            }

            return 0;
        }

        static void FromFil(string from, string to, NativeImageFormat formatFrom, Type formatTo)
        {
            Fil fil = Fil.FromFile(from);
            NativeImage ni = new NativeImage(fil.Data, new FormatHint(Path.GetExtension(from)));
            BitmapSource bmp = formatFrom.FromNative(ni);

            BitmapEncoder enc = (BitmapEncoder)Activator.CreateInstance(formatTo);
            enc.Frames.Add(BitmapFrame.Create(bmp));
            using (FileStream fs = new FileStream(to, FileMode.Create))
            {
                enc.Save(fs);
            }
        }

        static void ToFil(string from, string to, NativeImageFormat format, FilType type, bool dither)
        {
            BitmapImage bmp = new BitmapImage(new Uri(from, UriKind.Relative));
            EncodingOptions options = new EncodingOptions();
            options.Dither = dither;
            byte[] pixels = format.ToNative(bmp, options).Data;
            var fil = new Fil(Path.GetFileNameWithoutExtension(to));
            fil.Type = type;
            fil.Data = pixels;
            using (var fs = new FileStream(to, FileMode.Create))
            {
                fil.Write(fs);
            }
        }

        static void Usage(OptionSet options)
        {
            string exe = Process.GetCurrentProcess().ProcessName;
            System.Console.WriteLine("Usage: {0} [options] <infile> <outfile>", exe);
            System.Console.WriteLine("The file extensions determine the action and the output format.");
            System.Console.WriteLine("Options:");
            options.WriteOptionDescriptions(System.Console.Out);
        }

        static NativeImageFormat GetFormatByName(string name)
        {
            foreach (var f in _agatFormats)
            {
                if (string.Compare(f.Item2, name, true) == 0)
                {
                    return f.Item1;
                }
            }
            return null;
        }

        static readonly Tuple<NativeImageFormat, string>[] _agatFormats =
        {
            Tuple.Create((NativeImageFormat)new Gr7ImageFormat(), "GR7"),
            Tuple.Create((NativeImageFormat)new MgrImageFormat(), "MGR"),
            Tuple.Create((NativeImageFormat)new HgrImageFormat(), "HGR"),
            Tuple.Create((NativeImageFormat)new Mgr9ImageFormat(), "MGR9"),
            Tuple.Create((NativeImageFormat)new Hgr9ImageFormat(), "HGR9"),
        };

        static readonly Dictionary<string, Type> _extToFormat =
            new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".png", typeof(PngBitmapEncoder) },
            { ".jpg", typeof(JpegBitmapEncoder) },
            { ".gif", typeof(GifBitmapEncoder) },
            { ".bmp", typeof(BmpBitmapEncoder) },
        };
    }
}
