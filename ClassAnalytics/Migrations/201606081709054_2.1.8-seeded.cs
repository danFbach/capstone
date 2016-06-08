namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _218seeded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassModels",
                c => new
                    {
                        class_Id = c.Int(nullable: false, identity: true),
                        className = c.String(),
                        program_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.class_Id)
                .ForeignKey("dbo.ProgramModels", t => t.program_id, cascadeDelete: false)
                .Index(t => t.program_id);
            
            CreateTable(
                "dbo.ProgramModels",
                c => new
                    {
                        program_Id = c.Int(nullable: false, identity: true),
                        programName = c.String(),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.program_Id);
            
            CreateTable(
                "dbo.CourseModels",
                c => new
                    {
                        course_Id = c.Int(nullable: false, identity: true),
                        courseName = c.String(),
                        startDate = c.String(nullable: false),
                        endDate = c.String(nullable: false),
                        program_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.course_Id)
                .ForeignKey("dbo.ProgramModels", t => t.program_Id, cascadeDelete: false)
                .Index(t => t.program_Id);
            
            CreateTable(
                "dbo.GradeBookModels",
                c => new
                    {
                        grade_Id = c.Int(nullable: false, identity: true),
                        student_Id = c.Int(nullable: false),
                        task_Id = c.Int(nullable: false),
                        class_Id = c.Int(nullable: false),
                        assignment_notes = c.String(),
                        grade = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.grade_Id)
                .ForeignKey("dbo.ClassModels", t => t.class_Id, cascadeDelete: true)
                .ForeignKey("dbo.StudentModels", t => t.student_Id, cascadeDelete: true)
                .ForeignKey("dbo.TaskModels", t => t.task_Id, cascadeDelete: false)
                .Index(t => t.student_Id)
                .Index(t => t.task_Id)
                .Index(t => t.class_Id);
            
            CreateTable(
                "dbo.StudentModels",
                c => new
                    {
                        student_Id = c.Int(nullable: false, identity: true),
                        fName = c.String(nullable: false),
                        lName = c.String(nullable: false),
                        class_Id = c.Int(nullable: false),
                        student_account_Id = c.String(),
                    })
                .PrimaryKey(t => t.student_Id)
                .ForeignKey("dbo.ClassModels", t => t.class_Id, cascadeDelete: false)
                .Index(t => t.class_Id);
            
            CreateTable(
                "dbo.TaskModels",
                c => new
                    {
                        task_Id = c.Int(nullable: false, identity: true),
                        taskName = c.String(),
                        taskType_Id = c.Int(nullable: false),
                        points = c.Int(nullable: false),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        unit_Id = c.Int(nullable: false),
                        taskNotes = c.String(),
                    })
                .PrimaryKey(t => t.task_Id)
                .ForeignKey("dbo.TaskTypeModels", t => t.taskType_Id, cascadeDelete: false)
                .ForeignKey("dbo.UnitModels", t => t.unit_Id, cascadeDelete: true)
                .Index(t => t.taskType_Id)
                .Index(t => t.unit_Id);
            
            CreateTable(
                "dbo.TaskTypeModels",
                c => new
                    {
                        taskType_Id = c.Int(nullable: false, identity: true),
                        taskType = c.String(),
                        taskWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.taskType_Id);
            
            CreateTable(
                "dbo.UnitModels",
                c => new
                    {
                        unit_Id = c.Int(nullable: false, identity: true),
                        unitName = c.String(),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.unit_Id)
                .ForeignKey("dbo.CourseModels", t => t.course_Id, cascadeDelete: true)
                .Index(t => t.course_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SurveyModels",
                c => new
                    {
                        survey_Id = c.Int(nullable: false, identity: true),
                        SurveyName = c.String(),
                        surveyDate = c.DateTime(nullable: false),
                        active = c.Boolean(nullable: false),
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
                        survey_Id = c.Int(),
                    })
                .PrimaryKey(t => t.question_Id)
                .ForeignKey("dbo.SurveyModels", t => t.survey_Id)
                .Index(t => t.survey_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SurveyModels", "student_Id", "dbo.StudentModels");
            DropForeignKey("dbo.SurveyQuestions", "survey_Id", "dbo.SurveyModels");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.GradeBookModels", "task_Id", "dbo.TaskModels");
            DropForeignKey("dbo.TaskModels", "unit_Id", "dbo.UnitModels");
            DropForeignKey("dbo.UnitModels", "course_Id", "dbo.CourseModels");
            DropForeignKey("dbo.TaskModels", "taskType_Id", "dbo.TaskTypeModels");
            DropForeignKey("dbo.GradeBookModels", "student_Id", "dbo.StudentModels");
            DropForeignKey("dbo.StudentModels", "class_Id", "dbo.ClassModels");
            DropForeignKey("dbo.GradeBookModels", "class_Id", "dbo.ClassModels");
            DropForeignKey("dbo.CourseModels", "program_Id", "dbo.ProgramModels");
            DropForeignKey("dbo.ClassModels", "program_id", "dbo.ProgramModels");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SurveyQuestions", new[] { "survey_Id" });
            DropIndex("dbo.SurveyModels", new[] { "student_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.UnitModels", new[] { "course_Id" });
            DropIndex("dbo.TaskModels", new[] { "unit_Id" });
            DropIndex("dbo.TaskModels", new[] { "taskType_Id" });
            DropIndex("dbo.StudentModels", new[] { "class_Id" });
            DropIndex("dbo.GradeBookModels", new[] { "class_Id" });
            DropIndex("dbo.GradeBookModels", new[] { "task_Id" });
            DropIndex("dbo.GradeBookModels", new[] { "student_Id" });
            DropIndex("dbo.CourseModels", new[] { "program_Id" });
            DropIndex("dbo.ClassModels", new[] { "program_id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SurveyQuestions");
            DropTable("dbo.SurveyModels");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.UnitModels");
            DropTable("dbo.TaskTypeModels");
            DropTable("dbo.TaskModels");
            DropTable("dbo.StudentModels");
            DropTable("dbo.GradeBookModels");
            DropTable("dbo.CourseModels");
            DropTable("dbo.ProgramModels");
            DropTable("dbo.ClassModels");
        }
    }
}
