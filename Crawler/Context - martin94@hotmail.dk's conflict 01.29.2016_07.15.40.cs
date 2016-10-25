using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {

    internal class CrawlerContext : DbContext {

        public CrawlerContext() : base("name=CrawlerConnectionString") {
            Database.SetInitializer<CrawlerContext>(new DropCreateDatabaseIfModelChanges<CrawlerContext>());
        }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Link> Links { get; set; }
    }
}