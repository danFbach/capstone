namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tryingstudentuploads : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.studentUploads",
                c => new
                    {
                        file_Id = c.String(nullable: false, maxLength: 128),
                        student_account_id = c.String(),
                        class_name = c.String(),
                        file_name = c.String(),
                        createDate = c.DateTime(nullable: false),
                        task_id = c.Int(),
                    })
                .PrimaryKey(t => t.file_Id)
                .ForeignKey("dbo.TaskModels", t => t.task_id)
                .Index(t => t.task_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.studentUploads", "task_id", "dbo.TaskModels");
            DropIndex("dbo.studentUploads", new[] { "task_id" });
            DropTable("dbo.studentUploads");
        }
    }
}
