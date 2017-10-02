namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PositionCatalogs", "Specification_Id", "dbo.Specifications");
            DropIndex("dbo.PositionCatalogs", new[] { "Specification_Id" });
            CreateTable(
                "dbo.SpecificationPositionCatalogs",
                c => new
                    {
                        Specification_Id = c.Int(nullable: false),
                        PositionCatalog_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Specification_Id, t.PositionCatalog_Id })
                .ForeignKey("dbo.Specifications", t => t.Specification_Id, cascadeDelete: true)
                .ForeignKey("dbo.PositionCatalogs", t => t.PositionCatalog_Id, cascadeDelete: true)
                .Index(t => t.Specification_Id)
                .Index(t => t.PositionCatalog_Id);
            
            DropColumn("dbo.PositionCatalogs", "Specification_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PositionCatalogs", "Specification_Id", c => c.Int());
            DropForeignKey("dbo.SpecificationPositionCatalogs", "PositionCatalog_Id", "dbo.PositionCatalogs");
            DropForeignKey("dbo.SpecificationPositionCatalogs", "Specification_Id", "dbo.Specifications");
            DropIndex("dbo.SpecificationPositionCatalogs", new[] { "PositionCatalog_Id" });
            DropIndex("dbo.SpecificationPositionCatalogs", new[] { "Specification_Id" });
            DropTable("dbo.SpecificationPositionCatalogs");
            CreateIndex("dbo.PositionCatalogs", "Specification_Id");
            AddForeignKey("dbo.PositionCatalogs", "Specification_Id", "dbo.Specifications", "Id");
        }
    }
}
