namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PositionCatalogs", "Articul", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PositionCatalogs", "Articul");
        }
    }
}
