using NUnit.Framework;
using NeoAgi.CommandLine;
using NeoAgi.CommandLine.Tests.Models;
using System;

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

            OptBag? bag = null;

            try
            {
                bag = args.GetOpts<OptBag>();
            }
            catch (Exception)
            {
                // Gobble this up, may look into Assert.Catch() in time.
            }

            Assert.IsNull(bag, "Parameter Object is not Null");

            args[2] = "--option";

            bag = args.GetOpts<OptBag>();

            Assert.IsTrue(bag != null, "Parameter Object Is Null");

            Assert.Pass();
        }

        [Test]
        public void TestMismatchedArgList()
        {
            Assert.Pass();
        }
    }
}