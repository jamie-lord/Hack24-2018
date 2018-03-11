namespace SqlDataStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class creditcards : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCards",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    ImageUrl = c.String(),
                    AnnualFee = c.Double(nullable: false),
                    PurchaseApr = c.Double(nullable: false),
                    Link = c.String(),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.CreditCards");
        }
    }
}
