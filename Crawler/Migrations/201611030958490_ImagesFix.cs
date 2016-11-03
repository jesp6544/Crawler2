namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImagesFix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false, maxLength: 500, unicode: false),
                        Alt = c.String(nullable: false, maxLength: 500),
                        Page_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pages", t => t.Page_id)
                .Index(t => t.Path, unique: true)
                .Index(t => t.Alt)
                .Index(t => t.Page_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Images", "Page_id", "dbo.Pages");
            DropIndex("dbo.Images", new[] { "Page_id" });
            DropIndex("dbo.Images", new[] { "Alt" });
            DropIndex("dbo.Images", new[] { "Path" });
            DropTable("dbo.Images");
        }
    }
}
