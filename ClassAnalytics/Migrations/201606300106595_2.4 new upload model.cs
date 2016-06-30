namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24newuploadmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadModels", "createDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.UploadModels", "course_Id", c => c.Int());
            CreateIndex("dbo.UploadModels", "course_Id");
            AddForeignKey("dbo.UploadModels", "course_Id", "dbo.CourseModels", "course_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadModels", "course_Id", "dbo.CourseModels");
            DropIndex("dbo.UploadModels", new[] { "course_Id" });
            DropColumn("dbo.UploadModels", "course_Id");
            DropColumn("dbo.UploadModels", "createDate");
        }
    }
}
