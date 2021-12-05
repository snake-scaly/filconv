using System;
using System.IO;
using System.Linq;
using Xunit;

namespace FilLib.Tests
{
    public class FilTests
    {
        // FIL header with original name "TEST" and type T.
        private readonly byte[] _stubHeader =
        {
            0xD4, 0xC5, 0xD3, 0xD4, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        // A single sector of data.
        private readonly byte[] _stubData = new byte[256];

        public FilTests()
        {
            // Initialize sectors to a known data pattern.
            for (var i = 0; i < _stubData.Length; i++)
                _stubData[i] = (byte)i;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(32)]
        public void Read_Success(byte typeCode)
        {
            _stubHeader[39] = typeCode;
            using var stream = new MemoryStream(_stubHeader.Concat(_stubData).ToArray());

            var fil = Fil.Read(stream);

            Assert.NotNull(fil);
            Assert.Equal("TEST", fil.Name);
            Assert.Equal(new FilType(typeCode), fil.Type);
            Assert.Equal(_stubData, fil.Sectors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(32)]
        public void Read_Empty(byte typeCode)
        {
            _stubHeader[39] = typeCode;
            using var stream = new MemoryStream(_stubHeader);

            var fil = Fil.Read(stream);

            Assert.NotNull(fil);
            Assert.Equal("TEST", fil.Name);
            Assert.Equal(new FilType(typeCode), fil.Type);
            Assert.NotNull(fil.Sectors);
            Assert.Empty(fil.Sectors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(32)]
        public void ReadWriteNonDestructive(byte typeCode)
        {
            _stubHeader[39] = typeCode;
            new byte[] { 0, 32, 42, 0 }.CopyTo(_stubData, 0); // short B file metadata; only matters for typeCode 4
            var inStream = new MemoryStream(_stubHeader.Concat(_stubData).ToArray());

            var fil = Fil.Read(inStream);
            var outStream = new MemoryStream();
            fil.Write(outStream);
            var filBytes = outStream.ToArray();

            Assert.Equal(_stubHeader.Concat(_stubData), filBytes);
        }

        [Fact]
        public void CanCreateEmpty()
        {
            var fil = new Fil();

            Assert.Null(fil.Name);
            Assert.Null(fil.OriginalName);
            Assert.Equal(new FilType(0), fil.Type);
            Assert.NotNull(fil.Sectors);
            Assert.Empty(fil.Sectors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(32)]
        public void Write_Constructed(byte typeCode)
        {
            var fil = new Fil { Name = "TEST", Type = new FilType(typeCode), Sectors = _stubData };

            var outStream = new MemoryStream();
            fil.Write(outStream);
            var filBytes = outStream.ToArray();

            _stubHeader[39] = typeCode;
            Assert.Equal(_stubHeader.Concat(_stubData), filBytes);
        }

        [Fact]
        public void Write_Empty()
        {
            var fil = new Fil();

            var outStream = new MemoryStream();
            fil.Write(outStream);
            var filBytes = outStream.ToArray();

            for (var i = 0; i < 30; i++)
                _stubHeader[i] = 0xA0;
            Assert.Equal(_stubHeader, filBytes);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        public void GetData_SameAsSectorsForNonTypeB(byte typeCode)
        {
            _stubHeader[39] = typeCode;
            var inStream = new MemoryStream(_stubHeader.Concat(_stubData).ToArray());

            var fil = Fil.Read(inStream);

            Assert.Equal(_stubData, fil.GetData());
        }

        [Fact]
        public void GetData_ReturnsLoadablePartForTypeB()
        {
            _stubHeader[39] = 4;
            new byte[] { 0, 32, 42, 0 }.CopyTo(_stubData, 0);
            var inStream = new MemoryStream(_stubHeader.Concat(_stubData).ToArray());

            var fil = Fil.Read(inStream);

            Assert.Equal(_stubData.Skip(4).Take(42), fil.GetData());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(32)]
        public void GetData_ReturnsCopy(byte typeCode)
        {
            _stubHeader[39] = typeCode;
            new byte[] { 0, 32, 42, 0 }.CopyTo(_stubData, 0);
            var inStream = new MemoryStream(_stubHeader.Concat(_stubData).ToArray());

            var fil = Fil.Read(inStream);
            var data = fil.GetData();
            for (var i = 0; i < data.Length; i++)
                data[i] = 0xFF;

            Assert.Equal(_stubData, fil.Sectors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        public void SetData_UpdatesSectors(byte typeCode)
        {
            var fil = new Fil { Type = new FilType(typeCode) };
            fil.SetData(_stubData.Take(42).ToArray());

            Assert.Equal(_stubData.Take(42), fil.Sectors.Take(42));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        public void SetData_CreatesWholeSectors(byte typeCode)
        {
            var fil = new Fil { Type = new FilType(typeCode) };

            fil.SetData(Enumerable.Empty<byte>().ToArray());
            Assert.Empty(fil.Sectors);

            fil.SetData(new byte[42]);
            Assert.Equal(256, fil.Sectors.Length);

            fil.SetData(new byte[256]);
            Assert.Equal(256, fil.Sectors.Length);

            fil.SetData(new byte[257]);
            Assert.Equal(512, fil.Sectors.Length);
        }

        [Fact]
        public void SetData_UpdatesDataSizeTypeB()
        {
            var fil = new Fil { Type = new FilType(0x84) };
            fil.SetData(_stubData.Take(42).ToArray());

            Assert.Equal(42, fil.DataSize);

            var expected = new byte[256];
            new byte[] { 0, 0, 42, 0 }.CopyTo(expected, 0);
            _stubData.Take(42).ToArray().CopyTo(expected, 4);
            Assert.Equal(expected, fil.Sectors);
        }

        [Theory]
        [InlineData(0, 4, 256)]
        [InlineData(42, 46, 256)]
        [InlineData(252, 256, 256)]
        [InlineData(256, 260, 512)]
        public void SetData_TypeBSectorAndUnusedSize(int dataSize, int expectedEndOfData, int expectedSectorsSize)
        {
            var fil = new Fil { Type = new FilType(0x84) };
            fil.SetData(Enumerable.Repeat<byte>(0xFF, dataSize).ToArray());

            Assert.Equal(expectedSectorsSize, fil.Sectors.Length);
            if (dataSize > 0)
                Assert.Equal(0xFF, fil.Sectors[expectedEndOfData - 1]);
            if (expectedEndOfData < expectedSectorsSize)
                Assert.Equal(0, fil.Sectors[expectedEndOfData]);
        }

        [Fact]
        public void SetData_PreservesLoadAddressInTypeB()
        {
            var fil = new Fil { Type = new FilType(0x84) };
            fil.Sectors = new byte[1024];
            fil.LoadAddress = 0x2000;

            fil.SetData(_stubData);

            Assert.Equal(0x2000, fil.LoadAddress);
        }

        [Fact]
        public void LoadAddress_ReflectsSectorsOffset0ForTypeB()
        {
            var fil = new Fil { Type = new FilType(0x84) };

            fil.LoadAddress = 0x2000;
            Assert.Equal(256, fil.Sectors.Length);
            Assert.Equal(0, fil.Sectors[0]);
            Assert.Equal(32, fil.Sectors[1]);

            fil.Sectors = (byte[])_stubData.Clone();
            fil.LoadAddress = 0x3412;
            Assert.Equal(0x12, fil.Sectors[0]);
            Assert.Equal(0x34, fil.Sectors[1]);
            Assert.Equal(_stubData.Skip(2), fil.Sectors.Skip(2));

            fil.Sectors[0] = 24;
            fil.Sectors[1] = 1;
            Assert.Equal(0x118, fil.LoadAddress);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        public void LoadAddress_ReturnsDefaultForNonTypeB(byte typeCode)
        {
            var fil = new Fil { Type = new FilType(typeCode), Sectors = _stubData };

            Assert.Equal(0x2000, fil.LoadAddress);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        public void LoadAddress_ThrowsIfSetOnNonTypeB(byte typeCode)
        {
            var fil = new Fil { Type = new FilType(typeCode) };

            Assert.Throws<InvalidOperationException>(() => { fil.LoadAddress = 0x2000; });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(32)]
        public void DataSize_SameAsSectorsSizeForNonB(byte typeCode)
        {
            var fil = new Fil { Type = new FilType(typeCode), Sectors = new byte[42] };

            Assert.Equal(42, fil.DataSize);
        }

        [Fact]
        public void DataSize_ReflectsSectorsOffset2ForTypeB()
        {
            var fil = new Fil { Type = new FilType(0x84) };

            Assert.Equal(0, fil.DataSize);

            fil.SetData(_stubData.Take(42).ToArray());
            Assert.Equal(256, fil.Sectors.Length);
            Assert.Equal(42, fil.DataSize);
            Assert.Equal(42, fil.Sectors[2]);
            Assert.Equal(0, fil.Sectors[3]);

            fil.Sectors = (byte[])_stubData.Clone();
            Assert.Equal(0x302, fil.DataSize);
        }

        [Fact]
        public void Sectors_CannotBeNull()
        {
            var fil = new Fil();

            Assert.Throws<ArgumentNullException>(() => { fil.Sectors = null; });
        }
    }
}
