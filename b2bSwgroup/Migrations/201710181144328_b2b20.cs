namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PositionSpecifications", "Articul", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PositionSpecifications", "Articul");
        }
    }
}
