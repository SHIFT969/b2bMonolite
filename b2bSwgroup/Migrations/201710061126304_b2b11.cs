namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "Zakazchik", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "Zakazchik");
        }
    }
}
