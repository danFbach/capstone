namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class classTask : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassTaskJoinModels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        class_id = c.Int(nullable: false),
                        task_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ClassModels", t => t.class_id, cascadeDelete: true)
                .ForeignKey("dbo.TaskModels", t => t.task_id, cascadeDelete: true)
                .Index(t => t.class_id)
                .Index(t => t.task_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClassTaskJoinModels", "task_id", "dbo.TaskModels");
            DropForeignKey("dbo.ClassTaskJoinModels", "class_id", "dbo.ClassModels");
            DropIndex("dbo.ClassTaskJoinModels", new[] { "task_id" });
            DropIndex("dbo.ClassTaskJoinModels", new[] { "class_id" });
            DropTable("dbo.ClassTaskJoinModels");
        }
    }
}
