namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedindexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Errors", new[] { "Page_id" });
            CreateIndex("dbo.Pages", new[] { "scanned", "LastAttempt" }, name: "IX_AttemptAndScanned");
            CreateIndex("dbo.Errors", "Page_id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Errors", new[] { "Page_id" });
            DropIndex("dbo.Pages", "IX_AttemptAndScanned");
            CreateIndex("dbo.Errors", "Page_id");
        }
    }
}
