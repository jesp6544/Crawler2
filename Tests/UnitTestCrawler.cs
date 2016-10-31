using Crawler;
using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class UnitTestCrawler
    {
        [Test, Description("Testing connectivity to database.")]
        public void DatabaseConnectionTest()
        {
            var ctx = new CrawlerContext();
            Assert.IsNotNull(ctx.Database);
            Assert.IsNotNull(ctx.Content);
            Assert.IsNotNull(ctx.Errors);
            Assert.IsNotNull(ctx.Links);
            Assert.IsNotNull(ctx.Pages);
        }

        [Test, Description("Initialize Crawler object.")]
        public void InitializeCrawlerTest()
        {
            var crawler = new Crawler.Crawler();
            Assert.IsNotNull(crawler);
        }

        [Test, Description("Retrieving next page from database.")]
        public void GetNextPageTest()
        {
            var crawler = new Crawler.Crawler();
            crawler.CurrentPage = crawler.GetNextPage();
            Assert.IsNotNull(crawler.CurrentPage);
        }
    }
}