namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentuploadfixv236 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.studentUploads");
            AddColumn("dbo.studentUploads", "id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.studentUploads", "id");
            DropColumn("dbo.studentUploads", "file_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.studentUploads", "file_Id", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.studentUploads");
            DropColumn("dbo.studentUploads", "id");
            AddPrimaryKey("dbo.studentUploads", "file_Id");
        }
    }
}
