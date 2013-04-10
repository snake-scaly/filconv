using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FilLib
{
    public struct FilType : IEquatable<FilType>
    {
        public static readonly FilType T = new FilType('T', 0);
        public static readonly FilType B = new FilType('B', 4, new byte[] { 0, 32, 0, 32 });
        public static readonly FilType K = new FilType('K', 32);

        static readonly FilType[] _allTypes = { T, B, K };

        byte[] _header;

        public char Name { get; private set; }
        public int Code { get; private set; }

        FilType(char name, int code, byte[] header = null)
            : this()
        {
            Name = name;
            Code = code;
            _header = header != null ? header : new byte[0];
        }

        public void SkipHeader(Stream stream)
        {
            stream.Seek(_header.Length, SeekOrigin.Current);
        }

        public void WriteDefaultHeader(Stream output)
        {
            output.Write(_header, 0, _header.Length);
        }

        public static FilType FromCode(int code)
        {
            code &= 0x7F; // bit 7 is a weird flag bit
            foreach (FilType ft in _allTypes)
            {
                if (ft.Code == code)
                    return ft;
            }
            throw new NotSupportedException("Unknown file type: " + code);
        }

        public static FilType FromName(char name)
        {
            foreach (FilType ft in _allTypes)
            {
                if (ft.Name == Char.ToUpper(name))
                    return ft;
            }
            throw new NotSupportedException("Unknown file type: " + name);
        }

        public override bool Equals(object obj)
        {
            if (obj is FilType)
                return Equals((FilType)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public bool Equals(FilType other)
        {
            return Code == other.Code;
        }

        public static bool operator ==(FilType a, FilType b)
        {
            return a.Code == b.Code;
        }

        public static bool operator !=(FilType a, FilType b)
        {
            return a.Code != b.Code;
        }
    }
}
