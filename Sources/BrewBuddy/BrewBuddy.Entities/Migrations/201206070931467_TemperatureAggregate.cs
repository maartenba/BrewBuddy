namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemperatureAggregate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemperatureAggregates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BrewId = c.Int(nullable: false),
                        When = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TemperatureAggregates");
        }
    }
}
