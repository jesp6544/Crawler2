namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contentfix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contents", "text", c => c.String(nullable: false, maxLength: 800));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contents", "text", c => c.String(nullable: false));
        }
    }
}
