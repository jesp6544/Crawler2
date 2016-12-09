using System.Data.Entity.Migrations;

namespace Crawler.Migrations
{
    public partial class test1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.testtests",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    bla = c.String()
                })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.testtests");
        }
    }
}