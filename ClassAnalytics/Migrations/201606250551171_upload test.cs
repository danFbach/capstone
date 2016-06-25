namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploadtest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UploadModels",
                c => new
                    {
                        upload_id = c.Int(nullable: false, identity: true),
                        uploadName = c.String(),
                        class_id = c.Int(),
                        filePath = c.String(),
                    })
                .PrimaryKey(t => t.upload_id)
                .ForeignKey("dbo.ClassModels", t => t.class_id)
                .Index(t => t.class_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadModels", "class_id", "dbo.ClassModels");
            DropIndex("dbo.UploadModels", new[] { "class_id" });
            DropTable("dbo.UploadModels");
        }
    }
}
