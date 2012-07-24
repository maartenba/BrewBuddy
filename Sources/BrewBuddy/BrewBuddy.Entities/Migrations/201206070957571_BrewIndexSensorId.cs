namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class BrewIndexSensorId : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Brews", "SensorId", false);
            CreateIndex("dbo.TemperatureAggregates", "BrewId", false);
        }

        public override void Down()
        {
            DropIndex("dbo.TemperatureAggregates", new[] { "BrewId" });
            DropIndex("dbo.Brews", new[] { "SensorId" });
        }
    }
}
