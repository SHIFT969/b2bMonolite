namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PositionCatalogs", new[] { "DistributorApplicationUser_Id" });
            DropColumn("dbo.PositionCatalogs", "DistributorApplicationUserId");
            RenameColumn(table: "dbo.PositionCatalogs", name: "DistributorApplicationUser_Id", newName: "DistributorApplicationUserId");
            AlterColumn("dbo.PositionCatalogs", "DistributorApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PositionCatalogs", "DistributorApplicationUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PositionCatalogs", new[] { "DistributorApplicationUserId" });
            AlterColumn("dbo.PositionCatalogs", "DistributorApplicationUserId", c => c.Int());
            RenameColumn(table: "dbo.PositionCatalogs", name: "DistributorApplicationUserId", newName: "DistributorApplicationUser_Id");
            AddColumn("dbo.PositionCatalogs", "DistributorApplicationUserId", c => c.Int());
            CreateIndex("dbo.PositionCatalogs", "DistributorApplicationUser_Id");
        }
    }
}
