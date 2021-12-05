using Xunit;

namespace FilLib.Tests
{
    public class FilTypeTests
    {
        [Fact]
        public void EqualityIsConsistentWithInequality()
        {
            var t1 = new FilType(1);
            var t2 = new FilType(1);
            var t3 = new FilType(2);

            Assert.True(t1 == t2);
            Assert.False(t1 != t2);

            Assert.False(t1 == t3);
            Assert.True(t1 != t3);

            Assert.False(null == t1);
            Assert.True(null != t1);

            Assert.False(t1 == null);
            Assert.True(t1 != null);
        }
    }
}
