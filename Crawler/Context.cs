using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    internal class CrawlerContext : DbContext
    {
        public DbSet<Page> Pages { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Link> Links { get; set; }

        public CrawlerContext() : base("Server=tcp:indexer.database.windows.net,1433;Initial Catalog=IndexerDB;Persist Security Info=False;User ID=asdfAdmin;Password=GhW-Z4x-v9Q-PNb;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;")  //to azure DB
        {
            Database.SetInitializer<CrawlerContext>(new DBInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            /*modelBuilder.Entity<Page>()
                .HasOptional(l => l.fromLinks)
                .WithOptionalDependent()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Page>()
                .HasOptional(l => l.toLinks)
                .WithOptionalDependent()
                .WillCascadeOnDelete(false);*/

            /*modelBuilder.Entity<Page>()
                .HasOptional(p => p.fromLinks)
                .WithMany()
                .HasForeignKey(l => l.fromLinks)
                .WillCascadeOnDelete(false);*/

            /*modelBuilder.Entity<Page>()
                .HasMany(p => p.fromLinks)
                .WithOptional(l => l.from)
                .HasForeignKey(l => l.from_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Page>()
                .HasMany(p => p.toLinks)
                .WithOptional(l => l.to)
                .HasForeignKey(l => l.to_id)
                .WillCascadeOnDelete(false);*/

            modelBuilder.Entity<Link>()
                .HasRequired(l => l.from)
                .WithMany(p => p.fromLinks)
                .HasForeignKey(l => l.from_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Link>()
                .HasOptional(l => l.to)
                .WithMany(p => p.toLinks)
                .HasForeignKey(l => l.to_id)
                .WillCascadeOnDelete(false);

            /*modelBuilder.Entity<Link>()
                        .HasOptional(l => l.from)
                        .WithMany(p => p.fromLinks)
                        .HasForeignKey(l => l.from_id)
                        .WillCascadeOnDelete(false);*/

            /*modelBuilder.Entity<Link>()
                        .HasOptional(l => l.to)
                        .WithMany(p => p.toLinks)
                        .HasForeignKey(l => l.to_id)
                        .WillCascadeOnDelete(false);*/
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                foreach (var eve in vex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateException dbu)
            {
                var exception = HandleDbUpdateException(dbu);
                throw exception;
            }
        }

        internal class DBInitializer : CreateDatabaseIfNotExists<CrawlerContext>
        {
            protected override void Seed(CrawlerContext ctx)
            {
                Page d = new Page() { url = "https://en.wikipedia.org/wiki/Main_Page" };
                ctx.Entry(d).State = EntityState.Added;
                base.Seed(ctx);
            }
        }

        private Exception HandleDbUpdateException(DbUpdateException dbu)
        {
            var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");

            try
            {
                foreach (var result in dbu.Entries)
                {
                    builder.AppendFormat("Type: {0} was part of the problem. ", result.Entity.GetType().Name);
                }
            }
            catch (Exception e)
            {
                builder.Append("Error parsing DbUpdateException: " + e.ToString());
            }

            string message = builder.ToString();
            return new Exception(message, dbu);
        }
    }
}