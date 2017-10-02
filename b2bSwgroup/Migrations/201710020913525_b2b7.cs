namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "Name");
        }
    }
}
