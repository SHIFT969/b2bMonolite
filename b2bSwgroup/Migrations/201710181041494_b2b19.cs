namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Currencies", "СultureInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Currencies", "СultureInfo");
        }
    }
}
