namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class BrewStatusIsNowAnEnum : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Brews", "Status");
            AddColumn("dbo.Brews", "StatusValue", c => c.Int(nullable: false, defaultValue: 0));
        }

        public override void Down()
        {
            DropColumn("dbo.Brews", "StatusValue");
            AddColumn("dbo.Brews", "Status", c => c.String(maxLength: 255));
        }
    }
}
