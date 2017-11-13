namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shemas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DistributorId = c.Int(),
                        Sheet = c.Int(nullable: false),
                        FistRow = c.Int(nullable: false),
                        KeyColumn = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.DistributorId)
                .Index(t => t.DistributorId);
            
            CreateTable(
                "dbo.ShemaMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShemaId = c.Int(),
                        Name = c.String(nullable: false),
                        Column = c.Int(nullable: false),
                        DefaultValue = c.String(),
                        ParentLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shemas", t => t.ShemaId)
                .Index(t => t.ShemaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShemaMembers", "ShemaId", "dbo.Shemas");
            DropForeignKey("dbo.Shemas", "DistributorId", "dbo.Organizations");
            DropIndex("dbo.ShemaMembers", new[] { "ShemaId" });
            DropIndex("dbo.Shemas", new[] { "DistributorId" });
            DropTable("dbo.ShemaMembers");
            DropTable("dbo.Shemas");
        }
    }
}
