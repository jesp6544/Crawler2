using CrawlerLibrary.Models;
using CrawlerMVC.Models;

namespace CrawlerMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CrawlerMVC.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CrawlerMVC.Models.ApplicationDbContext";
        }

        protected override void Seed(CrawlerMVC.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            using (var ctx_mvc = new ApplicationDbContext())
            {
                if (ctx_mvc.SlaveControls.Any() == false) // Create initial dummy SlaveControl item for a fresh db
                {
                    ctx_mvc.SlaveControls.Add(new SlaveControl { TimeStamp = DateTime.Now });
                    ctx_mvc.SaveChanges();
                }
            }

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