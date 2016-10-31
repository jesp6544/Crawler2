using Crawler;
using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class UnitTestCrawler
    {
        [Test, Description("Testing connectivity to the database.")]
        public void DatabaseConnectionTest() // TODO
        {
            var ctx = new CrawlerContext();
            Assert.IsNotNull(ctx.Database);
            Assert.IsNotNull(ctx.Content);
            Assert.IsNotNull(ctx.Errors);
            Assert.IsNotNull(ctx.Links);
            Assert.IsNotNull(ctx.Pages);
        }

        [Test]
        public void InitializeCrawlerTest() // Real tests begin!
        {
            var crawler = new Crawler.Crawler();
            Assert.IsNotNull(crawler);
        }

        [Test]
        public void GetNextPageTest()
        {
            var crawler = new Crawler.Crawler();
            crawler.GetNextPage();
            Assert.IsNotNull(crawler.CurrentPage); // TODO: Never passes
        }
    }
}