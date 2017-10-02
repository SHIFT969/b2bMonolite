namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b6 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Specifications", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Specifications", "ApplicationUserId");
            RenameColumn(table: "dbo.Specifications", name: "ApplicationUser_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.Specifications", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Specifications", "ApplicationUserId");
            DropColumn("dbo.Specifications", "CustomerApplUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Specifications", "CustomerApplUserId", c => c.Int());
            DropIndex("dbo.Specifications", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Specifications", "ApplicationUserId", c => c.Int());
            RenameColumn(table: "dbo.Specifications", name: "ApplicationUserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.Specifications", "ApplicationUserId", c => c.Int());
            CreateIndex("dbo.Specifications", "ApplicationUser_Id");
        }
    }
}
