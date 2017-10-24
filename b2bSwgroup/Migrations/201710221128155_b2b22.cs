namespace b2bSwgroup.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b2b22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DateRegistration", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateLastLogin", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DateLastLogin");
            DropColumn("dbo.AspNetUsers", "DateRegistration");
        }
    }
}
