﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilLib;
using ImageLib;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using NDesk.Options;
using System.IO;

namespace filconv
{
    class Program
    {
        private const string filExtension = ".fil";

        static int Main(string[] args)
        {
            string filFormatName = "mgr";
            string filTypeName = "B";

            var options = new OptionSet()
            {
                {
                    "g|gfx-format=",
                    "Agat image format, MGR or HGR (case-insensitive).  Default is MGR",
                    v => {
                        v = v.ToLower();
                        if (v != "mgr" && v != "hgr")
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

            ImageLib.ImageFormat filFormat;
            switch (filFormatName)
            {
                case "mgr":
                    filFormat = new ImageLib.MgrImageFormat();
                    break;
                case "hgr":
                    filFormat = new ImageLib.HgrImageFormat();
                    break;
                default:
                    throw new Exception(string.Format("Invalid image format specified: '{0}'", filFormatName));
            }

            string src = positional[0];
            string dst = positional[1];

            if (src.ToLower().EndsWith(filExtension))
            {
                // converting from Agat to PC
                string ext = Path.GetExtension(dst);
                System.Drawing.Imaging.ImageFormat imgFormat;
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

        static void FromFil(string from, string to, ImageLib.ImageFormat formatFrom, System.Drawing.Imaging.ImageFormat formatTo)
        {
            Fil fil = Fil.FromFile(from);
            Bitmap bmp = ImageLib.ImageConverter.GetBitmap(fil.Data, formatFrom);
            bmp.Save(to, formatTo);
        }

        static void ToFil(string from, string to, ImageLib.ImageFormat format, FilType type)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(from);
            var pixels = ImageLib.ImageConverter.GetBytes(bmp, format);
            var fil = new Fil(Path.GetFileNameWithoutExtension(to));
            fil.Type = type;
            fil.Data = pixels;
            fil.Write(new FileStream(to, FileMode.Create));
        }

        static void Usage(OptionSet options)
        {
            string exe = Process.GetCurrentProcess().ProcessName;
            System.Console.WriteLine("Usage: {0} [options] <infile> <outfile>", exe);
            System.Console.WriteLine("The file extensions determine the action and the output format.");
            System.Console.WriteLine("Options:");
            options.WriteOptionDescriptions(System.Console.Out);
        }

        static readonly Dictionary<string, System.Drawing.Imaging.ImageFormat> _extToFormat =
            new Dictionary<string, System.Drawing.Imaging.ImageFormat>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".png", System.Drawing.Imaging.ImageFormat.Png },
            { ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg },
            { ".gif", System.Drawing.Imaging.ImageFormat.Gif },
            { ".bmp", System.Drawing.Imaging.ImageFormat.Bmp },
        };
    }
}
