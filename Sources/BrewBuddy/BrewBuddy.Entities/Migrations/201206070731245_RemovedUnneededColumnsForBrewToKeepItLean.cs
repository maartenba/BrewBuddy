namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedUnneededColumnsForBrewToKeepItLean : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Brews", "Started");
            DropColumn("dbo.Brews", "Finished");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Brews", "Finished", c => c.DateTime());
            AddColumn("dbo.Brews", "Started", c => c.DateTime());
        }
    }
}
