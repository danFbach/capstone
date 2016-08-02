namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class surveystructuresimplification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SurveyJoinTableModels", "class_Id", "dbo.ClassModels");
            DropForeignKey("dbo.SurveyJoinTableModels", "survey_Id", "dbo.SurveyModels");
            DropForeignKey("dbo.SurveyAnswers", "survey_join_id", "dbo.SurveyJoinTableModels");
            DropIndex("dbo.SurveyAnswers", new[] { "survey_join_id" });
            DropIndex("dbo.SurveyJoinTableModels", new[] { "class_Id" });
            DropIndex("dbo.SurveyJoinTableModels", new[] { "survey_Id" });
            AddColumn("dbo.SurveyModels", "class_id", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyModels", "active", c => c.Boolean(nullable: false));
            CreateIndex("dbo.SurveyModels", "class_id");
            AddForeignKey("dbo.SurveyModels", "class_id", "dbo.ClassModels", "class_Id", cascadeDelete: true);
            DropColumn("dbo.SurveyAnswers", "survey_join_id");
            DropTable("dbo.SurveyJoinTableModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SurveyJoinTableModels",
                c => new
                    {
                        survey_join_Id = c.Int(nullable: false, identity: true),
                        class_Id = c.Int(nullable: false),
                        survey_Id = c.Int(nullable: false),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.survey_join_Id);
            
            AddColumn("dbo.SurveyAnswers", "survey_join_id", c => c.Int(nullable: false));
            DropForeignKey("dbo.SurveyModels", "class_id", "dbo.ClassModels");
            DropIndex("dbo.SurveyModels", new[] { "class_id" });
            DropColumn("dbo.SurveyModels", "active");
            DropColumn("dbo.SurveyModels", "class_id");
            CreateIndex("dbo.SurveyJoinTableModels", "survey_Id");
            CreateIndex("dbo.SurveyJoinTableModels", "class_Id");
            CreateIndex("dbo.SurveyAnswers", "survey_join_id");
            AddForeignKey("dbo.SurveyAnswers", "survey_join_id", "dbo.SurveyJoinTableModels", "survey_join_Id", cascadeDelete: true);
            AddForeignKey("dbo.SurveyJoinTableModels", "survey_Id", "dbo.SurveyModels", "survey_Id", cascadeDelete: true);
            AddForeignKey("dbo.SurveyJoinTableModels", "class_Id", "dbo.ClassModels", "class_Id", cascadeDelete: true);
        }
    }
}
