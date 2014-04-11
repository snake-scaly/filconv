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
        public const int DefauldLoadingAddress = 0x2000;

        /// <summary>
        /// Decoded name of the file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Name of the file in original encoding
        /// </summary>
        public byte[] OriginalName { get; private set; }

        public FilType Type { get; set; }

        public UInt16 StartAddress { get; set; }

        public byte[] Data { get; set; }

        private Fil()
        {
            StartAddress = DefauldLoadingAddress;
        }

        public Fil(string name)
            : this()
        {
            Name = name.Length > MaxNameLength ? name.Substring(0, MaxNameLength) : name;
            Type = FilType.B;
        }

        private Fil(byte[] originalName)
            : this()
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

            int sizeLimit = -1;

            if (fil.Type == FilType.B)
            {
                fil.StartAddress = binaryReader.ReadUInt16();
                sizeLimit = binaryReader.ReadUInt16();
            }
            
            var ms = new MemoryStream();
            byte[] buf = new byte[8192];
            int count;
            while ((count = input.Read(buf, 0, buf.Length)) != 0)
            {
                ms.Write(buf, 0, count);
            }
            fil.Data = ms.ToArray();

            if (sizeLimit != -1 && fil.Data.Length > sizeLimit)
            {
                var sub = new byte[sizeLimit];
                Array.Copy(fil.Data, sub, sizeLimit);
                fil.Data = sub;
            }
            
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

            if (Type == FilType.B)
            {
                binaryWriter.Write(StartAddress);
                binaryWriter.Write((UInt16)Data.Length);
            }

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
