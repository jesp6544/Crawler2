using System.Data.Entity.Migrations;

namespace Crawler.Migrations
{
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