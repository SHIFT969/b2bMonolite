namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "Comment");
        }
    }
}
