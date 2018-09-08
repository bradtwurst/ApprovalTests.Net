﻿using ApprovalTests.Namers;
using NUnit.Framework;

namespace ApprovalTests.Tests.Namer
{
    [TestFixture]
    public class AdditionalInformationTests
    {
        [Test]
        public void ExitsBeforeNamerIsCalled()
        {
            using (ApprovalResults.UniqueForMachineName())
            {
            }
        }

        [Test]
        public void UniqueForRuntime()
        {
            using (ApprovalResults.UniqueForRuntime())
            {
                Approvals.Verify(ApprovalResults.GetDotNetRuntime(true));
            }
        }

        [Test]
        public void WithoutExtraInfo()
        {
            var name = Approvals.GetDefaultNamer().Name;
            Assert.AreEqual("AdditionalInformationTests.WithoutExtraInfo", name);
        }

        [Test]
        public void WithScenarioData()
        {
            using (ApprovalResults.ForScenario("scenarioname"))
            {
                var name = Approvals.GetDefaultNamer().Name;
                Assert.AreEqual("AdditionalInformationTests.WithScenarioData.ForScenario.scenarioname", name);
            }
        }


        [Test]
        public void WithScenarioDataScrubsInvalidChars()
        {
            using (ApprovalResults.ForScenario("invalid/chars"))
            {
                var name = Approvals.GetDefaultNamer().Name;
                Assert.AreEqual(
                    "AdditionalInformationTests.WithScenarioDataScrubsInvalidChars.ForScenario.invalid_chars", name);
            }
        }

        [Test]
        [TestCase("foo", "bar")]
        public void WithMultiplePartScenarioData(string a, string b)
        {
            using (ApprovalResults.ForScenario(a, b))
            {
                var name = Approvals.GetDefaultNamer().Name;
                Assert.AreEqual("AdditionalInformationTests.WithMultiplePartScenarioData.ForScenario.foo.bar", name);
            }
        }

        [Test]
        public void TestMultipleNames()
        {
            using (ApprovalResults.ForScenario("scenario"))
            {
                using (ApprovalResults.ForScenario("machineName"))
                {
                    var name = Approvals.GetDefaultNamer().Name;
                    Assert.AreEqual(name,
                        "AdditionalInformationTests.TestMultipleNames.ForScenario.scenario.ForScenario.machineName");
                }
            }
        }

        [TestFixture]
        public class NestedClassTests
        {
            [Test]
            public void WithNestedClass()
            {
                var name = Approvals.GetDefaultNamer().Name;
                Assert.AreEqual("AdditionalInformationTests.NestedClassTests.WithNestedClass", name);
            }
        }
    }
}