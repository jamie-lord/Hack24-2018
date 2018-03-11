namespace SqlDataStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creditcards2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditCards", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CreditCards", "Name");
        }
    }
}
