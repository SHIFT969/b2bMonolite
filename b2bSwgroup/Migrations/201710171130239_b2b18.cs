namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "DateCreate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Specifications", "DateEdit", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "DateEdit");
            DropColumn("dbo.Specifications", "DateCreate");
        }
    }
}
