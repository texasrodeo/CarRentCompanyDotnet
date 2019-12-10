namespace CarRentCompanyDotnet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyMigration3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contracts", "ClientId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contracts", "ClientId", c => c.Int(nullable: false));
        }
    }
}
