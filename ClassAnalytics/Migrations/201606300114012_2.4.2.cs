namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _242 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadModels", "uploadType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UploadModels", "uploadType");
        }
    }
}
