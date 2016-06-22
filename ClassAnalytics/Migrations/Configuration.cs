namespace ClassAnalytics.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ClassAnalytics.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ClassAnalytics.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ClassAnalytics.Models.ApplicationDbContext context)
        {
            //context.programModels.AddOrUpdate(x => x.program_Id,
            //    new ProgramModels { programName = "15 Week Session", startDate = Convert.ToDateTime("06/07/2017"), endDate = Convert.ToDateTime("10/07/2017") },
            //    new ProgramModels { programName = "Night Session", startDate = Convert.ToDateTime("06/20/2017"), endDate = Convert.ToDateTime("08/07/2017") }
            //);
            //context.TaskTypeModels.AddOrUpdate(x => x.taskType_Id,
            //    new TaskTypeModels { taskType = "Test", taskWeight = 40 },
            //    new TaskTypeModels { taskType = "Assignment", taskWeight = 20 },
            //    new TaskTypeModels { taskType = "Project", taskWeight = 40 }
            //);
            //context.Roles.AddOrUpdate(x => x.Id,
            //   new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "Admin" },
            //   new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "New Student" },
            //   new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "Student" }
            //);
            //context.classmodel.AddOrUpdate(x => x.class_Id,
            ////    new ClassModel { className = "Flourine", program_id = 1 },
            ////    new ClassModel { className = "Oxygen", program_id = 1 },
            ////    new ClassModel { className = "Magnesium", program_id = 1 },
            //    new ClassModel { className = "Gold", program_id = 1 },
            //    new ClassModel { className = "Plutonium", program_id = 2 }
            ////    new ClassModel { className = "Barium", program_id = 2 }
            //);
            //context.coursemodels.AddOrUpdate(x => x.course_Id,
            ////    new CourseModels { courseName = "Python", startDate = "06/07/2017", endDate = "06/12/2017", program_Id = 1 },
            //    new CourseModels { courseName = "C#", startDate = "06/22/2017", endDate = "06/27/2017", program_Id = 1 },
            //    new CourseModels { courseName = "JavaScript", startDate = "06/15/2017", endDate = "06/20/2017", program_Id = 1 },
            //    new CourseModels { courseName = "Python", startDate = "06/07/2017", endDate = "06/12/2017", program_Id = 2 }
            //);
            //context.taskModel.AddOrUpdate(x => x.task_Id,
            //    new TaskModel { taskType_Id = 1, taskName = "Python test", startDate = Convert.ToDateTime("06/12/2017"), endDate = Convert.ToDateTime("06/12/2017"), points = 40, course_Id = 3, taskNotes = "a test" },
            //    new TaskModel { taskType_Id = 2, taskName = "Binary Search Tree", startDate = Convert.ToDateTime("06/07/2017"), endDate = Convert.ToDateTime("06/09/2017"), points = 30, course_Id = 2, taskNotes = "" },
            //    new TaskModel { taskType_Id = 3, taskName = "Lemonade Stand", startDate = Convert.ToDateTime("06/09/2017"), endDate = Convert.ToDateTime("06/12/2017"), points = 30, course_Id = 1, taskNotes = "" }
            //    );
            //context.surveyModel.AddOrUpdate(x => x.survey_Id,
            //    new SurveyModel { SurveyName = "Python Progress", course_Id = 1 },
            //    new SurveyModel { SurveyName = "C# Progress", course_Id = 2 }
            //);
            //context.surveyQuestion.AddOrUpdate(x => x.question_Id,
            //    new SurveyQuestion { question = "Question 1", survey_Id = 1 },
            //    new SurveyQuestion { question = "Question 2", survey_Id = 1 },
            //    new SurveyQuestion { question = "Question 3", survey_Id = 1 },
            //    new SurveyQuestion { question = "Question 4", survey_Id = 1 },
            //    new SurveyQuestion { question = "Question 5", survey_Id = 1 },
            //    new SurveyQuestion { question = "Question 1", survey_Id = 2 },
            //    new SurveyQuestion { question = "Question 2", survey_Id = 2 },
            //    new SurveyQuestion { question = "Question 3", survey_Id = 2 },
            //    new SurveyQuestion { question = "Question 4", survey_Id = 2 },
            //    new SurveyQuestion { question = "Question 5", survey_Id = 2 }
            //    );
            //context.surveyJoinTableModel.AddOrUpdate(x => x.survey_join_Id,
            //    new SurveyJoinTableModel { class_Id = 1, survey_Id = 1, active = false },
            //    new SurveyJoinTableModel { class_Id = 1, survey_Id = 2, active = false },
            //    new SurveyJoinTableModel { class_Id = 2, survey_Id = 1, active = false },
            //    new SurveyJoinTableModel { class_Id = 2, survey_Id = 2, active = false },
            //    );
        }
    }
}
