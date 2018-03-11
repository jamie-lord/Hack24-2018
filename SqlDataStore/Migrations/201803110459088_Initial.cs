namespace SqlDataStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    FromId = c.String(),
                    FromName = c.String(),
                    RecipientId = c.String(),
                    RecipientName = c.String(),
                    TextFormat = c.String(),
                    TopicName = c.String(),
                    HistoryDisclosed = c.Boolean(nullable: false),
                    Local = c.String(),
                    Text = c.String(),
                    Summary = c.String(),
                    ChannelId = c.String(),
                    ServiceUrl = c.String(),
                    ReplyToId = c.String(),
                    Action = c.String(),
                    Type = c.String(),
                    Timestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    ConversationId = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UserDetails",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    Name = c.String(),
                    Email = c.String(),
                    PhoneNumber = c.Double(nullable: false),
                    JobTitle = c.String(),
                    Location = c.String(),
                    Salary = c.Double(nullable: false),
                    YearsOfXp = c.Int(),
                    Age = c.Int(),
                    Gender = c.Int(),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.UserDetails");
            DropTable("dbo.Activities");
        }
    }
}
