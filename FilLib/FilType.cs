using System;

namespace FilLib
{
    public class FilType : IEquatable<FilType>
    {
        private readonly int _typeIndex;

        public FilType(byte code)
        {
            Code = code;
            _typeIndex = CalculateTypeIndex(code);
        }

        public byte Code { get; }

        public bool HasAddrSize => _typeIndex == 3;

        public static FilType FromName(char name, bool protect = false)
        {
            var typeCode = NameToCode(name) | (protect ? 0x80 : 0);
            return new FilType((byte)typeCode);
        }

        public override bool Equals(object obj) => obj is FilType other && Equals(other);
        public bool Equals(FilType other) => Code == other?.Code;
        public override int GetHashCode() => Code.GetHashCode();

        public static bool operator ==(FilType a, FilType b)
        {
            if (a is null)
                return b is null;
            return a.Equals(b);
        }

        public static bool operator !=(FilType a, FilType b) => !(a == b);

        // Calculate type index like Apple DOS does:
        // 1. Ignore the most significant write-protect bit.
        // 2. Find the most significant non-zero bit.
        // 3. If found, return its one-based index in the byte.
        // 4. If not found, return zero which is type T.
        private static int CalculateTypeIndex(int code)
        {
            for (int mask = 0x40, i = 7; mask != 0; mask >>= 1, i--)
                if ((code & mask) != 0)
                    return i;
            return 0;
        }

        private static int NameToCode(char name)
        {
            switch (name)
            {
                case 'T': return 0;
                case 'I': return 1;
                case 'A': return 2;
                case 'B': return 4;
                case 'S': return 8;
                case 'R': return 16;
                case 'П': return 16;
                case 'К': return 32;
                case 'Д': return 64;
                default: throw new NotSupportedException("Unknown file type: " + name);
            }
        }
    }
}
