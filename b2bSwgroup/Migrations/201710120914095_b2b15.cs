namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b15 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PositionSpecificationSpecifications", "PositionSpecification_Id", "dbo.PositionSpecifications");
            DropForeignKey("dbo.PositionSpecificationSpecifications", "Specification_Id", "dbo.Specifications");
            DropIndex("dbo.PositionSpecificationSpecifications", new[] { "PositionSpecification_Id" });
            DropIndex("dbo.PositionSpecificationSpecifications", new[] { "Specification_Id" });
            AddColumn("dbo.PositionSpecifications", "SpecificationId", c => c.Int());
            CreateIndex("dbo.PositionSpecifications", "SpecificationId");
            AddForeignKey("dbo.PositionSpecifications", "SpecificationId", "dbo.Specifications", "Id");
            DropTable("dbo.PositionSpecificationSpecifications");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PositionSpecificationSpecifications",
                c => new
                    {
                        PositionSpecification_Id = c.Int(nullable: false),
                        Specification_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PositionSpecification_Id, t.Specification_Id });
            
            DropForeignKey("dbo.PositionSpecifications", "SpecificationId", "dbo.Specifications");
            DropIndex("dbo.PositionSpecifications", new[] { "SpecificationId" });
            DropColumn("dbo.PositionSpecifications", "SpecificationId");
            CreateIndex("dbo.PositionSpecificationSpecifications", "Specification_Id");
            CreateIndex("dbo.PositionSpecificationSpecifications", "PositionSpecification_Id");
            AddForeignKey("dbo.PositionSpecificationSpecifications", "Specification_Id", "dbo.Specifications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PositionSpecificationSpecifications", "PositionSpecification_Id", "dbo.PositionSpecifications", "Id", cascadeDelete: true);
        }
    }
}
