namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentuploadmodified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.studentUploads", "student_id", c => c.Int(nullable: false));
            CreateIndex("dbo.studentUploads", "student_id");
            AddForeignKey("dbo.studentUploads", "student_id", "dbo.StudentModels", "student_Id", cascadeDelete: true);
            DropColumn("dbo.studentUploads", "student_account_id");
            DropColumn("dbo.studentUploads", "class_name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.studentUploads", "class_name", c => c.String());
            AddColumn("dbo.studentUploads", "student_account_id", c => c.String());
            DropForeignKey("dbo.studentUploads", "student_id", "dbo.StudentModels");
            DropIndex("dbo.studentUploads", new[] { "student_id" });
            DropColumn("dbo.studentUploads", "student_id");
        }
    }
}
