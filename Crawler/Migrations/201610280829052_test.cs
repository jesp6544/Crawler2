using System.Data.Entity.Migrations;

namespace Crawler.Migrations
{
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contents",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    tag = c.String(nullable: false),
                    text = c.String(nullable: false),
                    page_id = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pages", t => t.page_id, cascadeDelete: true)
                .Index(t => t.page_id);

            CreateTable(
                "dbo.Pages",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    url = c.String(nullable: false, maxLength: 500, unicode: false),
                    title = c.String(),
                    LastAttempt = c.DateTime(),
                    scanned = c.Boolean()
                })
                .PrimaryKey(t => t.id)
                .Index(t => t.url, unique: true);

            CreateTable(
                "dbo.Links",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    text = c.String(nullable: false),
                    local = c.Boolean(nullable: false),
                    from_id = c.Int(nullable: false),
                    to_id = c.Int()
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pages", t => t.from_id)
                .ForeignKey("dbo.Pages", t => t.to_id)
                .Index(t => t.from_id)
                .Index(t => t.to_id);

            CreateTable(
                "dbo.Errors",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    error = c.String(),
                    time = c.DateTime(nullable: false),
                    Page_id = c.Int()
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pages", t => t.Page_id)
                .Index(t => t.Page_id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Errors", "Page_id", "dbo.Pages");
            DropForeignKey("dbo.Links", "to_id", "dbo.Pages");
            DropForeignKey("dbo.Links", "from_id", "dbo.Pages");
            DropForeignKey("dbo.Contents", "page_id", "dbo.Pages");
            DropIndex("dbo.Errors", new[] { "Page_id" });
            DropIndex("dbo.Links", new[] { "to_id" });
            DropIndex("dbo.Links", new[] { "from_id" });
            DropIndex("dbo.Pages", new[] { "url" });
            DropIndex("dbo.Contents", new[] { "page_id" });
            DropTable("dbo.Errors");
            DropTable("dbo.Links");
            DropTable("dbo.Pages");
            DropTable("dbo.Contents");
        }
    }
}