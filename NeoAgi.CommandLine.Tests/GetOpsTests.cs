using Microsoft.Extensions.Configuration;
using NeoAgi.CommandLine.Exceptions;
using NeoAgi.CommandLine.Tests.Models;
using NUnit.Framework;
using System;

namespace NeoAgi.CommandLine.Tests
{
    public class Tests
    {
        protected static string[] argsForOptionBagAllRequired = new string[] { "--location", "path/to/blah", "--option", "some value" };
        protected static string[] argsForOptionBagMostRequired = new string[] { "--location", "/path/to/blah" };
        protected static string[] argsForOptionBagAllRequiredWithEquals = new string[] { "--location=path/to/blah", "--option", "some value" };
        protected static string[] argsForOptionBagAllRequiredWithValueless = new string[] { "--location=path/to/blah", "-v", "--option", "some value", "--dry-run" };

        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "General Test for GetOps<T> functionality.")]
        public void TestGetOps()
        {
            OptBag? bag = null;

            var exception = Assert.Throws<CommandLineOptionParseException>(() =>
            {
                bag = argsForOptionBagMostRequired.GetOpts<OptBag>();
            });

            // Try this once more, in a positive path
            bag = argsForOptionBagAllRequired.GetOpts<OptBag>();

            Assert.That(bag != null, "Options were not processed.");
            Assert.That(bag!.Location.Equals(argsForOptionBagAllRequired[1], StringComparison.Ordinal), $"OptBag.Location was not correct.  Expected {argsForOptionBagAllRequired[1]}, received {bag.Location}");
            Assert.That(bag!.Opt.Equals(argsForOptionBagAllRequired[3], StringComparison.Ordinal), $"OptBag.Opt was not correct.  Expected {argsForOptionBagAllRequired[3]}, received {bag.Opt}");

            Assert.Pass();
        }

        [Test(Description = "Tests the result of going down the OptionManager.Merge<T>() which is used to process input from a string[].")]
        public void TestGetOpsThrowRequired()
        {
            OptBag? bag = null;

            // Assert that an exception will be thrown
            var exception = Assert.Throws<CommandLineOptionParseException>(() =>
            {
                bag = argsForOptionBagMostRequired.GetOpts<OptBag>();
            });

            // Assert the exception was thrown due to missing a required variable
            Assert.That(exception!.OptionsWithErrors.Count > 0);
            Assert.That(exception.OptionsWithErrors[0].Option.Required, $"Nested Option Required exception expected but not found.  Reson received {exception.OptionsWithErrors[0].Option.Required}.");
        }

        [Test(Description = "Tests the result of going down the OptionManager.Flatten<T>() which is used with Microsoft.Extensions.Configuration.")]
        public void TestGetOpsThrowRequiredConfiguration()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .AddOpts<OptBag>(argsForOptionBagMostRequired);

            var exception = Assert.Throws<CommandLineOptionParseException>(() =>
            {
                IConfigurationRoot root = configBuilder.Build();
            });

            // Assert the exception was thrown due to missing a required variable
            Assert.That(exception!.OptionsWithErrors.Count > 0);
            Assert.That(exception.OptionsWithErrors[0].Option.Required, $"Nested Option Required exception expected but not found.  Reson received {exception.OptionsWithErrors[0].Option.Required}.");
        }

        [Test(Description = "Test Args Parsing with '=' options parsing.")]
        public void TestGetOpsEqualsSeparator()
        {
            OptBag bag = argsForOptionBagAllRequiredWithEquals.GetOpts<OptBag>();

            Assert.That(bag, Is.Not.Null, "Option Bag was not populated");
            Assert.That(bag.Location.Equals(argsForOptionBagAllRequired[1]), $"Location was not parsed correctly.  Expected '{argsForOptionBagAllRequired[1]}', received '{bag.Location}'.");
        }

        [Test(Description = "Test Args Parsing of Valueless/Flags.")]
        public void TestGetOpsValuelessFlags()
        {
            OptBag bag = argsForOptionBagAllRequiredWithValueless.GetOpts<OptBag>();

            Assert.That(bag, Is.Not.Null, "Option Bag was not populated");
            Assert.That(bag.DryRun, $"Dry Run flag (final-arg) was not parsed correctly.");
            Assert.That(bag.Verbosity, $"Verbosity flag (mid-arg) was not parsed correctly.");
        }

        [Test]
        public void TestMismatchedArgList()
        {
            Assert.Pass();
        }
    }
}