namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class survey : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SurveyModels",
                c => new
                    {
                        survey_Id = c.Int(nullable: false, identity: true),
                        SurveyName = c.String(),
                        surveyDate = c.DateTime(nullable: true),
                        active = c.Boolean(nullable: false, defaultValue:true),
                        student_Id = c.Int(),
                    })
                .PrimaryKey(t => t.survey_Id)
                .ForeignKey("dbo.StudentModels", t => t.student_Id)
                .Index(t => t.student_Id);
            
            CreateTable(
                "dbo.SurveyQuestions",
                c => new
                    {
                        question_Id = c.Int(nullable: false, identity: true),
                        question = c.String(),
                        answer = c.Boolean(nullable: false),
                        survey_Id = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.question_Id)
                .ForeignKey("dbo.SurveyModels", t => t.survey_Id, cascadeDelete: true)
                .Index(t => t.survey_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyModels", "student_Id", "dbo.StudentModels");
            DropForeignKey("dbo.SurveyQuestions", "survey_Id", "dbo.SurveyModels");
            DropIndex("dbo.SurveyQuestions", new[] { "survey_Id" });
            DropIndex("dbo.SurveyModels", new[] { "student_Id" });
            DropTable("dbo.SurveyQuestions");
            DropTable("dbo.SurveyModels");
        }
    }
}
