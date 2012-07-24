namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Brew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipeId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 255),
                        Title = c.String(nullable: false, maxLength: 255),
                        Annotations = c.String(maxLength: 4000),
                        IsPublic = c.Boolean(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        Status = c.String(maxLength: 255),
                        Started = c.DateTime(nullable: false),
                        Finished = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .Index(t => t.RecipeId)
                .Index(t => t.UserName);
        }

        public override void Down()
        {
            DropIndex("dbo.Brews", new[] { "RecipeId" });
            DropIndex("dbo.Brews", new[] { "UserName" });
            DropForeignKey("dbo.Brews", "RecipeId", "dbo.Recipes");
            DropTable("dbo.Brews");
        }
    }
}
