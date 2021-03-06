using CrawlerLibrary.Models;
using System.Data.Entity.Migrations;

namespace Crawler.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<CrawlerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            CommandTimeout = 60 * 5;
        }

        protected override void Seed(CrawlerContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}