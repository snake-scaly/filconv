using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

            string agatFormats = string.Join(", ", _agatFormats.Select(f => f.Name));

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

            AgatImageFormat filFormat = GetFormatByName(filFormatName);

            string src = positional[0];
            string dst = positional[1];

            if (src.ToLower().EndsWith(filExtension))
            {
                // converting from Agat to PC
                string ext = Path.GetExtension(dst);
                ImageFormat imgFormat;
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
                ToFil(src, dst, filFormat, FilType.FromName(filTypeName[0]));
            }
            else
            {
                System.Console.Error.WriteLine("Neither of files have a {0} extension", filExtension);
                return 1;
            }

            return 0;
        }

        static void FromFil(string from, string to, AgatImageFormat formatFrom, ImageFormat formatTo)
        {
            Fil fil = Fil.FromFile(from);
            Bitmap bmp = AgatImageConverter.GetBitmap(fil.Data, formatFrom);
            bmp.Save(to, formatTo);
        }

        static void ToFil(string from, string to, AgatImageFormat format, FilType type)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(from);
            var pixels = AgatImageConverter.GetBytes(bmp, format);
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

        static AgatImageFormat GetFormatByName(string name)
        {
            foreach (AgatImageFormat f in _agatFormats)
            {
                if (string.Compare(f.Name, name, true) == 0)
                {
                    return f;
                }
            }
            return null;
        }

        static readonly AgatImageFormat[] _agatFormats =
        {
            new Gr7ImageFormat(),
            new MgrImageFormat(),
            new HgrImageFormat(),
            new Mgr9ImageFormat(),
            new Hgr9ImageFormat(),
        };

        static readonly Dictionary<string, ImageFormat> _extToFormat =
            new Dictionary<string, ImageFormat>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".png", ImageFormat.Png },
            { ".jpg", ImageFormat.Jpeg },
            { ".gif", ImageFormat.Gif },
            { ".bmp", ImageFormat.Bmp },
        };
    }
}
