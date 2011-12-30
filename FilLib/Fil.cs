using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;

namespace FilLib
{
    public class Fil
    {
        public const int MaxNameLength = 30;

        /// <summary>
        /// Decoded name of the file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Name of the file in original encoding
        /// </summary>
        public byte[] OriginalName { get; private set; }

        public FilType Type { get; set; }

        public byte[] Data { get; set; }

        public Fil(string name)
        {
            Name = name;
            Type = FilType.B;
        }

        private Fil(byte[] originalName)
        {
            OriginalName = originalName;
            Name = AgatEncoding.Decode(originalName).Trim();
        }

        public static Fil Read(Stream input)
        {
            var binaryReader = new BinaryReader(input);

            byte[] originalName = binaryReader.ReadBytes(MaxNameLength);
            var fil = new Fil(originalName);
            
            binaryReader.BaseStream.Seek(9, SeekOrigin.Current);

            int t = binaryReader.ReadByte();
            fil.Type = FilType.FromCode(t);

            fil.Type.SkipHeader(input);
            
            var ms = new MemoryStream();
            byte[] buf = new byte[8192];
            int count;
            while ((count = input.Read(buf, 0, buf.Length)) != 0)
            {
                ms.Write(buf, 0, count);
            }
            fil.Data = ms.ToArray();
            
            return fil;
        }

        public void Write(Stream output)
        {
            var binaryWriter = new BinaryWriter(output);

            byte[] name = OriginalName;
            if (name == null)
            {
                name = AgatEncoding.Encode(string.Format("{0,-30}", Name));
            }
            binaryWriter.Write(name);

            binaryWriter.Seek(9, SeekOrigin.Current);
            binaryWriter.Write((byte)Type.Code);
            Type.WriteDefaultHeader(output);

            binaryWriter.Write(Data);
        }

        public static Fil FromFile(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                return Read(fs);
            }
        }
    }
}
