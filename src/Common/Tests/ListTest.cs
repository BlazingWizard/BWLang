using BW.Common.DataStruct;
using Xunit;
using Xunit.Abstractions;

namespace BW.Common.Tests
{
    public class ListTest
    {
        private readonly ITestOutputHelper output;

        public ListTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private BWList<int> Init()
        {
            var list = new BWList<int>();

            list.Add(10);
            list.Add(20);

            
           
            return list;
        }


        [Fact]
        public void AddTest()
        {
            var list = Init();
            Assert.Equal(2, list.Count);
            output.WriteLine(list.ToString());
        }

        [Fact]
        public void GetTest()
        {
            var list = Init();

            Assert.Equal(10, list.Get(0));
            Assert.Equal(20, list.Get(1));
        }

        [Fact]
        public void DeleteTest()
        {
            var list = Init();

            list.Delete(0);

            Assert.Equal(1, list.Count);
            Assert.Equal(20, list.Get(0));

            list.Delete(0);

            Assert.Equal(0, list.Count);
            output.WriteLine(list.ToString());
        }
    }
}
