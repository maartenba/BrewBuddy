namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSensorIdColumnToBrew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brews", "SensorId", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Brews", "SensorId");
        }
    }
}
