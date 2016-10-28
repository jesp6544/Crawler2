namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testtest : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.testtests");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.testtests",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        bla = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
    }
}
