namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "Activity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "Activity");
        }
    }
}
