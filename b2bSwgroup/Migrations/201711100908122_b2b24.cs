namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shemas", "IgnoreValue", c => c.String());
            AddColumn("dbo.Shemas", "IgnoreColumn", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shemas", "IgnoreColumn");
            DropColumn("dbo.Shemas", "IgnoreValue");
        }
    }
}
