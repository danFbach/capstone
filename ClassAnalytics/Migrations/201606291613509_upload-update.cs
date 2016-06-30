namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploadupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadModels", "active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UploadModels", "active");
        }
    }
}
