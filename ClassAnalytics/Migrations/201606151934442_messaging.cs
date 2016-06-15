namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messaging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InstructorModels",
                c => new
                    {
                        instuctor_Id = c.Int(nullable: false, identity: true),
                        fName = c.String(nullable: false),
                        lName = c.String(nullable: false),
                        instructor_account_Id = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.instuctor_Id);
            
            CreateTable(
                "dbo.MessagingModels",
                c => new
                    {
                        message_Id = c.Int(nullable: false, identity: true),
                        sender_account_Id = c.String(),
                        recipient_account_Id = c.String(),
                        message = c.String(),
                        read = c.Boolean(nullable: false),
                        dateSent = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.message_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MessagingModels");
            DropTable("dbo.InstructorModels");
        }
    }
}
