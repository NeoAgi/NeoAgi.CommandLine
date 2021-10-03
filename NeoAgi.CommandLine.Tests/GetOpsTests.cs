using NUnit.Framework;
using NeoAgi.CommandLine;

namespace NeoAgi.CommandLine.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetOps()
        {
            string[] args = new string[]
            {
                "--location",
                "path/to/blah",
                "--opt",
                "some value"
            };

            OptBag? bag = args.GetOps<OptBag>();

            Assert.IsNotNull(bag, "Parameter Object Is Null");

            Assert.Pass();
        }
    }
}