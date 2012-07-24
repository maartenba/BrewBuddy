namespace BrewBuddy.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecipeIsPublicProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "IsPublic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "IsPublic");
        }
    }
}
