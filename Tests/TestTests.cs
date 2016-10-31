using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    internal class TestTests
    {
        [Test]
        public void PositiveTestTest() // This test always passes
        {
            int x = 7;
            int y = 7;
            Assert.AreEqual(x, y);
        }

        [Test]
        public void NegativeTestTest() // This test always fails
        {
            if (true)
            {
                Assert.Fail();
            }
        }

        [Test, Ignore("Not yet implemented")]
        public void IgnoreTestTest() // This test is ignored
        {
        }
    }
}