namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrossCategories", "DistributorId", c => c.Int());
            AddColumn("dbo.CrossCategories", "IdentifyCategory", c => c.String());
            AddColumn("dbo.CrossCategories", "CategoryId", c => c.Int());
            CreateIndex("dbo.CrossCategories", "DistributorId");
            CreateIndex("dbo.CrossCategories", "CategoryId");
            AddForeignKey("dbo.CrossCategories", "CategoryId", "dbo.Categories", "Id");
            AddForeignKey("dbo.CrossCategories", "DistributorId", "dbo.Organizations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CrossCategories", "DistributorId", "dbo.Organizations");
            DropForeignKey("dbo.CrossCategories", "CategoryId", "dbo.Categories");
            DropIndex("dbo.CrossCategories", new[] { "CategoryId" });
            DropIndex("dbo.CrossCategories", new[] { "DistributorId" });
            DropColumn("dbo.CrossCategories", "CategoryId");
            DropColumn("dbo.CrossCategories", "IdentifyCategory");
            DropColumn("dbo.CrossCategories", "DistributorId");
        }
    }
}
