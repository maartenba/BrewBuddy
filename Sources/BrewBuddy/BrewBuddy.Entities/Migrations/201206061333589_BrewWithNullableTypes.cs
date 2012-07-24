namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BrewWithNullableTypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Brews", "Started", c => c.DateTime());
            AlterColumn("dbo.Brews", "Finished", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Brews", "Finished", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Brews", "Started", c => c.DateTime(nullable: false));
        }
    }
}
