namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploadcollumnfilePathchangedtorelativePath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadModels", "relativePath", c => c.String());
            DropColumn("dbo.UploadModels", "filePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UploadModels", "filePath", c => c.String());
            DropColumn("dbo.UploadModels", "relativePath");
        }
    }
}
