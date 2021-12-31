using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FilLib
{
    public class Fil
    {
        private string _name;
        private FilType _type = new FilType(0);
        private byte[] _sectors;

        public const int MaxNameLength = 30;
        public const int DefaultLoadAddress = 0x2000;

        public Fil()
        {
            Sectors = new byte[0];
        }

        private Fil(byte[] originalName)
        {
            OriginalName = originalName;
            Name = AgatEncoding.Decode(originalName).Trim();
        }

        /// <summary>
        /// Decoded name of the file
        /// </summary>
        public string Name
        {
            get => _name;

            set
            {
                _name = string.Concat(value.Take(MaxNameLength));
                OriginalName = null;
            }
        }

        /// <summary>
        /// Name of the file in original encoding
        /// </summary>
        public byte[] OriginalName { get; private set; }

        public FilType Type
        {
            get => _type;
            set => _type = value ?? throw new ArgumentNullException(nameof(value));
        }

        public byte[] Sectors
        {
            get => _sectors;
            set => _sectors = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ushort LoadAddress
        {
            get
            {
                if (!Type.HasAddrSize)
                    return DefaultLoadAddress;
                return BitConverter.ToUInt16(Sectors, 0);
            }

            set
            {
                if (!Type.HasAddrSize)
                    throw new InvalidOperationException($"Cannot set load address for file type {Type}");
                ReallocateSectors(Math.Max(2, _sectors.Length));
                BitConverter.GetBytes(value).CopyTo(Sectors, 0);
            }
        }

        public ushort DataSize
        {
            get
            {
                if (!Type.HasAddrSize)
                    return (ushort)Sectors.Length;
                if (Sectors.Length < 4)
                    return 0;
                return BitConverter.ToUInt16(Sectors, 2);
            }

            private set
            {
                Debug.Assert(Type.HasAddrSize);
                BitConverter.GetBytes(value).CopyTo(Sectors, 2);
            }
        }

        public static Fil Read(Stream input)
        {
            var binaryReader = new BinaryReader(input);

            var originalName = binaryReader.ReadBytes(MaxNameLength);
            var fil = new Fil(originalName);

            binaryReader.BaseStream.Seek(9, SeekOrigin.Current);

            var t = binaryReader.ReadByte();
            fil.Type = new FilType(t);

            using (var memoryStream = new MemoryStream())
            {
                binaryReader.BaseStream.CopyTo(memoryStream);
                fil.Sectors = memoryStream.ToArray();
            }

            return fil;
        }

        public void Write(Stream output)
        {
            var binaryWriter = new BinaryWriter(output);

            var name = OriginalName ?? AgatEncoding.Encode($"{Name,-30}");
            binaryWriter.Write(name);

            binaryWriter.Seek(9, SeekOrigin.Current);
            binaryWriter.Write(Type.Code);

            binaryWriter.Write(Sectors);
        }

        public static Fil FromFile(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                return Read(fs);
            }
        }

        public byte[] GetData()
        {
            if (!Type.HasAddrSize)
                return (byte[])Sectors.Clone();
            return Sectors.Skip(4).Take(DataSize).ToArray();
        }

        public void SetData(ICollection<byte> data)
        {
            if (Type.HasAddrSize)
            {
                ReallocateSectors(data.Count + 4);
                data.CopyTo(Sectors, 4);
                DataSize = (ushort)data.Count;
            }
            else
            {
                ReallocateSectors(data.Count);
                data.CopyTo(Sectors, 0);
            }
        }

        private void ReallocateSectors(int minSize)
        {
            var sectorsSize = (minSize + 255) / 256 * 256;
            if (sectorsSize == Sectors.Length)
                return;

            var newSectors = new byte[sectorsSize];
            Array.Copy(Sectors, 0, newSectors, 0, Math.Min(Sectors.Length, newSectors.Length));
            Sectors = newSectors;
        }
    }
}
