using System;
using System.Configuration;
using CrawlerLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SolrNet;
using System.Linq;
using Assert = NUnit.Framework.Assert;

namespace Crawler.Tests
{
    [TestFixture]
    public class UnitTestCrawler
    {
        [Test, NUnit.Framework.Description("Testing connectivity to database.")]
        public void DatabaseConnectionTest()
        {
            var ctx = new CrawlerContext();
            Assert.IsNotNull(ctx.Database);
            Assert.IsNotNull(ctx.Content);
            Assert.IsNotNull(ctx.Errors);
            Assert.IsNotNull(ctx.Links);
            Assert.IsNotNull(ctx.Pages);
        }

        [Test, NUnit.Framework.Description("Initialize Crawler object.")]
        public void InitializeCrawlerTest()
        {
            var crawler = new Crawler();
            Assert.IsNotNull(crawler);
        }

        [Test, NUnit.Framework.Description("Retrieving next page from database.")]
        public void GetNextPageTest()
        {
            var crawler = new Crawler();
            crawler.CurrentPage = crawler.GetNextPage();
            Assert.IsNotNull(crawler.CurrentPage);
        }
        [Test, NUnit.Framework.Description("Testing if broken links / internal links gets fixed")]
        public void FixLinkTest()
        {
            var crawler = new Crawler();
            PrivateObject obj = new PrivateObject(crawler);
            string bla = (string)obj.Invoke("FixLink",new object[]{"http://www.lort.com", "/martinErLort", true });
            string bla2 = (string)obj.Invoke("FixLink", new object[] { "http://www.lort.com", "//martinErLort", true });
            string bla3 = (string)obj.Invoke("FixLink", new object[] { "http://www.lort.com", "?martinErLort", true });
            Assert.AreEqual(bla, "http://www.lort.com/martinErLort");
            Assert.AreEqual(bla2, "http://www.lort.com/martinErLort");
            Assert.IsTrue(bla3 == "http://www.lort.com?martinErLort" || bla3 == "http://www.lort.com/?martinErLort");
        }

        [Test, NUnit.Framework.Description("Tests for connection to solr")]
        public void SolrTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var bla = ConfigurationManager.ConnectionStrings["Solr"].ConnectionString;
                Startup.InitContainer();
                Startup.Init<HTMLContent>(bla);
            });
        }
        [Test, NUnit.Framework.Description("")]
        public void xxTest()
        {
            
        }
    
    }
}