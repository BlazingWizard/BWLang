using BW.Common.DataStruct;

using Xunit;

namespace BW.Common.Tests
{
    public class HashSetTest
    {
        private BWHashSet<double> Init()
        {
            var hashSet = new BWHashSet<double>(100);

            hashSet.Add(100);
            hashSet.Add(45);

            return hashSet;
        }

        [Fact]
        public void ContsansTest()
        {
            var hashSet = Init();

            Assert.True(hashSet.Contains(45));
            Assert.False(hashSet.Contains(70));
        }
    }
}
