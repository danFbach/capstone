namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3 : DbMigration
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
                "dbo.ClassTaskJoinModels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        class_id = c.Int(nullable: false),
                        task_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ClassModels", t => t.class_id, cascadeDelete: true)
                .ForeignKey("dbo.TaskModels", t => t.task_id, cascadeDelete: false)
                .Index(t => t.class_id)
                .Index(t => t.task_id);
            
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
                        course_Id = c.Int(nullable: false),
                        taskNotes = c.String(),
                    })
                .PrimaryKey(t => t.task_Id)
                .ForeignKey("dbo.CourseModels", t => t.course_Id, cascadeDelete: true)
                .ForeignKey("dbo.TaskTypeModels", t => t.taskType_Id, cascadeDelete: false)
                .Index(t => t.taskType_Id)
                .Index(t => t.course_Id);
            
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
                .ForeignKey("dbo.ProgramModels", t => t.program_Id, cascadeDelete: true)
                .Index(t => t.program_Id);
            
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
                "dbo.GradeBookModels",
                c => new
                    {
                        grade_Id = c.Int(nullable: false, identity: true),
                        student_Id = c.Int(nullable: false),
                        task_Id = c.Int(nullable: false),
                        class_Id = c.Int(nullable: false),
                        assignment_notes = c.String(),
                        possiblePoints = c.Int(nullable: false),
                        pointsEarned = c.Decimal(precision: 18, scale: 2),
                        grade = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.grade_Id)
                .ForeignKey("dbo.ClassModels", t => t.class_Id, cascadeDelete: true)
                .ForeignKey("dbo.StudentModels", t => t.student_Id, cascadeDelete: true)
                .ForeignKey("dbo.TaskModels", t => t.task_Id, cascadeDelete: true)
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
                "dbo.InstrctrClassJoins",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        instructor_id = c.Int(nullable: false),
                        class_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ClassModels", t => t.class_id, cascadeDelete: true)
                .Index(t => t.class_id);
            
            CreateTable(
                "dbo.InstructorModels",
                c => new
                    {
                        instructor_Id = c.Int(nullable: false, identity: true),
                        fName = c.String(nullable: false),
                        lName = c.String(nullable: false),
                        email = c.String(nullable: false),
                        instructor_account_Id = c.String(),
                    })
                .PrimaryKey(t => t.instructor_Id);
            
            CreateTable(
                "dbo.MessagingModels",
                c => new
                    {
                        message_Id = c.Int(nullable: false, identity: true),
                        sending_id = c.String(),
                        recieving_id = c.String(),
                        message = c.String(),
                        subject = c.String(nullable: false),
                        read = c.Boolean(nullable: false),
                        dateSent = c.DateTime(nullable: false),
                        receiving_User_Id = c.String(maxLength: 128),
                        sending_User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.message_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.receiving_User_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.sending_User_Id)
                .Index(t => t.receiving_User_Id)
                .Index(t => t.sending_User_Id);
            
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
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                "dbo.studentUploads",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        student_id = c.Int(nullable: false),
                        file_name = c.String(),
                        createDate = c.DateTime(nullable: false),
                        task_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.StudentModels", t => t.student_id, cascadeDelete: true)
                .ForeignKey("dbo.TaskModels", t => t.task_id)
                .Index(t => t.student_id)
                .Index(t => t.task_id);
            
            CreateTable(
                "dbo.SurveyAnswers",
                c => new
                    {
                        answer_Id = c.Int(nullable: false, identity: true),
                        student_Id = c.Int(nullable: false),
                        answer = c.Boolean(nullable: false),
                        question_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.answer_Id)
                .ForeignKey("dbo.StudentModels", t => t.student_Id, cascadeDelete: true)
                .ForeignKey("dbo.SurveyQuestions", t => t.question_Id, cascadeDelete: true)
                .Index(t => t.student_Id)
                .Index(t => t.question_Id);
            
            CreateTable(
                "dbo.SurveyQuestions",
                c => new
                    {
                        question_Id = c.Int(nullable: false, identity: true),
                        question = c.String(),
                        survey_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.question_Id)
                .ForeignKey("dbo.SurveyModels", t => t.survey_Id, cascadeDelete: true)
                .Index(t => t.survey_Id);
            
            CreateTable(
                "dbo.SurveyModels",
                c => new
                    {
                        survey_Id = c.Int(nullable: false, identity: true),
                        SurveyName = c.String(),
                        course_Id = c.Int(nullable: false),
                        class_id = c.Int(nullable: false),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.survey_Id)
                .ForeignKey("dbo.ClassModels", t => t.class_id, cascadeDelete: true)
                .ForeignKey("dbo.CourseModels", t => t.course_Id, cascadeDelete: true)
                .Index(t => t.course_Id)
                .Index(t => t.class_id);
            
            CreateTable(
                "dbo.UploadModels",
                c => new
                    {
                        upload_id = c.Int(nullable: false, identity: true),
                        uploadName = c.String(),
                        class_id = c.Int(),
                        relativePath = c.String(),
                        active = c.Boolean(nullable: false),
                        createDate = c.DateTime(nullable: false),
                        uploadType = c.String(),
                        course_Id = c.Int(),
                    })
                .PrimaryKey(t => t.upload_id)
                .ForeignKey("dbo.ClassModels", t => t.class_id)
                .ForeignKey("dbo.CourseModels", t => t.course_Id)
                .Index(t => t.class_id)
                .Index(t => t.course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadModels", "course_Id", "dbo.CourseModels");
            DropForeignKey("dbo.UploadModels", "class_id", "dbo.ClassModels");
            DropForeignKey("dbo.SurveyAnswers", "question_Id", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyQuestions", "survey_Id", "dbo.SurveyModels");
            DropForeignKey("dbo.SurveyModels", "course_Id", "dbo.CourseModels");
            DropForeignKey("dbo.SurveyModels", "class_id", "dbo.ClassModels");
            DropForeignKey("dbo.SurveyAnswers", "student_Id", "dbo.StudentModels");
            DropForeignKey("dbo.studentUploads", "task_id", "dbo.TaskModels");
            DropForeignKey("dbo.studentUploads", "student_id", "dbo.StudentModels");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.MessagingModels", "sending_User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessagingModels", "receiving_User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InstrctrClassJoins", "class_id", "dbo.ClassModels");
            DropForeignKey("dbo.GradeBookModels", "task_Id", "dbo.TaskModels");
            DropForeignKey("dbo.GradeBookModels", "student_Id", "dbo.StudentModels");
            DropForeignKey("dbo.StudentModels", "class_Id", "dbo.ClassModels");
            DropForeignKey("dbo.GradeBookModels", "class_Id", "dbo.ClassModels");
            DropForeignKey("dbo.ClassTaskJoinModels", "task_id", "dbo.TaskModels");
            DropForeignKey("dbo.TaskModels", "taskType_Id", "dbo.TaskTypeModels");
            DropForeignKey("dbo.TaskModels", "course_Id", "dbo.CourseModels");
            DropForeignKey("dbo.CourseModels", "program_Id", "dbo.ProgramModels");
            DropForeignKey("dbo.ClassTaskJoinModels", "class_id", "dbo.ClassModels");
            DropForeignKey("dbo.ClassModels", "program_id", "dbo.ProgramModels");
            DropIndex("dbo.UploadModels", new[] { "course_Id" });
            DropIndex("dbo.UploadModels", new[] { "class_id" });
            DropIndex("dbo.SurveyModels", new[] { "class_id" });
            DropIndex("dbo.SurveyModels", new[] { "course_Id" });
            DropIndex("dbo.SurveyQuestions", new[] { "survey_Id" });
            DropIndex("dbo.SurveyAnswers", new[] { "question_Id" });
            DropIndex("dbo.SurveyAnswers", new[] { "student_Id" });
            DropIndex("dbo.studentUploads", new[] { "task_id" });
            DropIndex("dbo.studentUploads", new[] { "student_id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.MessagingModels", new[] { "sending_User_Id" });
            DropIndex("dbo.MessagingModels", new[] { "receiving_User_Id" });
            DropIndex("dbo.InstrctrClassJoins", new[] { "class_id" });
            DropIndex("dbo.StudentModels", new[] { "class_Id" });
            DropIndex("dbo.GradeBookModels", new[] { "class_Id" });
            DropIndex("dbo.GradeBookModels", new[] { "task_Id" });
            DropIndex("dbo.GradeBookModels", new[] { "student_Id" });
            DropIndex("dbo.CourseModels", new[] { "program_Id" });
            DropIndex("dbo.TaskModels", new[] { "course_Id" });
            DropIndex("dbo.TaskModels", new[] { "taskType_Id" });
            DropIndex("dbo.ClassTaskJoinModels", new[] { "task_id" });
            DropIndex("dbo.ClassTaskJoinModels", new[] { "class_id" });
            DropIndex("dbo.ClassModels", new[] { "program_id" });
            DropTable("dbo.UploadModels");
            DropTable("dbo.SurveyModels");
            DropTable("dbo.SurveyQuestions");
            DropTable("dbo.SurveyAnswers");
            DropTable("dbo.studentUploads");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.MessagingModels");
            DropTable("dbo.InstructorModels");
            DropTable("dbo.InstrctrClassJoins");
            DropTable("dbo.StudentModels");
            DropTable("dbo.GradeBookModels");
            DropTable("dbo.TaskTypeModels");
            DropTable("dbo.CourseModels");
            DropTable("dbo.TaskModels");
            DropTable("dbo.ClassTaskJoinModels");
            DropTable("dbo.ProgramModels");
            DropTable("dbo.ClassModels");
        }
    }
}
