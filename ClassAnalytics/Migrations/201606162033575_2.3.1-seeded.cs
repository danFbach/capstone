namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _231seeded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MessagingModels", "subject", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MessagingModels", "subject", c => c.String());
        }
    }
}
