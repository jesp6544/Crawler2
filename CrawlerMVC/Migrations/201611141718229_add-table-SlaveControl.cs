namespace CrawlerMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableSlaveControl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SlaveControls",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Stop = c.Boolean(),
                        Pause = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SlaveControls");
        }
    }
}
