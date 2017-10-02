namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PositionCatalogs", "DistributorId", "dbo.Organizations");
            DropIndex("dbo.PositionCatalogs", new[] { "DistributorId" });
            AlterColumn("dbo.PositionCatalogs", "DistributorId", c => c.Int());
            CreateIndex("dbo.PositionCatalogs", "DistributorId");
            AddForeignKey("dbo.PositionCatalogs", "DistributorId", "dbo.Organizations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PositionCatalogs", "DistributorId", "dbo.Organizations");
            DropIndex("dbo.PositionCatalogs", new[] { "DistributorId" });
            AlterColumn("dbo.PositionCatalogs", "DistributorId", c => c.Int(nullable: false));
            CreateIndex("dbo.PositionCatalogs", "DistributorId");
            AddForeignKey("dbo.PositionCatalogs", "DistributorId", "dbo.Organizations", "Id", cascadeDelete: true);
        }
    }
}
