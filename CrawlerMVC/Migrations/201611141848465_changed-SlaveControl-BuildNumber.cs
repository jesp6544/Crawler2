namespace CrawlerMVC.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class changedSlaveControlBuildNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SlaveControls", "BuildNumber", c => c.Int(nullable: false));
            AddColumn("dbo.SlaveControls", "TimeStamp", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.SlaveControls", "TimeStamp");
            DropColumn("dbo.SlaveControls", "BuildNumber");
        }
    }
}