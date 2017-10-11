namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SpecificationPositionCatalogs", "Specification_Id", "dbo.Specifications");
            DropForeignKey("dbo.SpecificationPositionCatalogs", "PositionCatalog_Id", "dbo.PositionCatalogs");
            DropIndex("dbo.SpecificationPositionCatalogs", new[] { "Specification_Id" });
            DropIndex("dbo.SpecificationPositionCatalogs", new[] { "PositionCatalog_Id" });
            CreateTable(
                "dbo.PositionSpecifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PartNumber = c.String(),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        CurrencyId = c.Int(),
                        CategoryId = c.Int(),
                        Quantity = c.Int(nullable: false),
                        DistributorId = c.Int(),
                        DistributorApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId)
                .ForeignKey("dbo.Organizations", t => t.DistributorId)
                .ForeignKey("dbo.AspNetUsers", t => t.DistributorApplicationUserId)
                .Index(t => t.CurrencyId)
                .Index(t => t.CategoryId)
                .Index(t => t.DistributorId)
                .Index(t => t.DistributorApplicationUserId);
            
            CreateTable(
                "dbo.PositionSpecificationSpecifications",
                c => new
                    {
                        PositionSpecification_Id = c.Int(nullable: false),
                        Specification_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PositionSpecification_Id, t.Specification_Id })
                .ForeignKey("dbo.PositionSpecifications", t => t.PositionSpecification_Id, cascadeDelete: true)
                .ForeignKey("dbo.Specifications", t => t.Specification_Id, cascadeDelete: true)
                .Index(t => t.PositionSpecification_Id)
                .Index(t => t.Specification_Id);
            
            AddColumn("dbo.PositionCatalogs", "Quantity", c => c.Int(nullable: false));
            DropTable("dbo.SpecificationPositionCatalogs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SpecificationPositionCatalogs",
                c => new
                    {
                        Specification_Id = c.Int(nullable: false),
                        PositionCatalog_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Specification_Id, t.PositionCatalog_Id });
            
            DropForeignKey("dbo.PositionSpecificationSpecifications", "Specification_Id", "dbo.Specifications");
            DropForeignKey("dbo.PositionSpecificationSpecifications", "PositionSpecification_Id", "dbo.PositionSpecifications");
            DropForeignKey("dbo.PositionSpecifications", "DistributorApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PositionSpecifications", "DistributorId", "dbo.Organizations");
            DropForeignKey("dbo.PositionSpecifications", "CurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.PositionSpecifications", "CategoryId", "dbo.Categories");
            DropIndex("dbo.PositionSpecificationSpecifications", new[] { "Specification_Id" });
            DropIndex("dbo.PositionSpecificationSpecifications", new[] { "PositionSpecification_Id" });
            DropIndex("dbo.PositionSpecifications", new[] { "DistributorApplicationUserId" });
            DropIndex("dbo.PositionSpecifications", new[] { "DistributorId" });
            DropIndex("dbo.PositionSpecifications", new[] { "CategoryId" });
            DropIndex("dbo.PositionSpecifications", new[] { "CurrencyId" });
            DropColumn("dbo.PositionCatalogs", "Quantity");
            DropTable("dbo.PositionSpecificationSpecifications");
            DropTable("dbo.PositionSpecifications");
            CreateIndex("dbo.SpecificationPositionCatalogs", "PositionCatalog_Id");
            CreateIndex("dbo.SpecificationPositionCatalogs", "Specification_Id");
            AddForeignKey("dbo.SpecificationPositionCatalogs", "PositionCatalog_Id", "dbo.PositionCatalogs", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SpecificationPositionCatalogs", "Specification_Id", "dbo.Specifications", "Id", cascadeDelete: true);
        }
    }
}
