namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialDatabaseVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 255, nullable: false),
                        Email = c.String(maxLength: 255),
                        Name = c.String(maxLength: 255),
                        Location = c.String(maxLength: 255),
                        Information = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 255, nullable: false),
                        Title = c.String(maxLength: 255, nullable: false),
                        Description = c.String(maxLength: 4000),
                        Ingredients = c.String(maxLength: 4000),
                        Instructions = c.String(maxLength: 4000),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.Recipes");
            DropTable("dbo.UserProfiles");
        }
    }
}
