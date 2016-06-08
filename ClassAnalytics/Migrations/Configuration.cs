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
            context.programModels.AddOrUpdate( x => x.program_Id,
                new ProgramModels { programName = "15 Week Session", startDate = Convert.ToDateTime("06/07/2017"), endDate = Convert.ToDateTime("10/07/2017") },
                new ProgramModels { programName = "Night Session", startDate = Convert.ToDateTime("06/20/2017"), endDate = Convert.ToDateTime("08/07/2017") }
            );
            context.coursemodels.AddOrUpdate(x => x.course_Id,
                new CourseModels { courseName = "Python", startDate = "06/07/2017", endDate = "06/12/2017", program_Id = 1 },
                new CourseModels { courseName = "C#", startDate = "06/22/2017", endDate = "06/27/2017", program_Id = 1 },
                new CourseModels { courseName = "JavaScript", startDate = "06/15/2017", endDate = "06/20/2017", program_Id = 1 },
                new CourseModels { courseName = "Python", startDate = "06/07/2017", endDate = "06/12/2017", program_Id = 2 }
            );
            context.unitModels.AddOrUpdate(x => x.unit_Id,
                new UnitModels { unitName = "Functional Programming", startDate = Convert.ToDateTime("06/07/2017"), endDate = Convert.ToDateTime("06/09/2017"), course_Id = 13 },
                new UnitModels { unitName = "OOP", startDate = Convert.ToDateTime("06/09/2017"), endDate = Convert.ToDateTime("06/12/2017"), course_Id = 13 },
                new UnitModels { unitName = "Functional Programming", startDate = Convert.ToDateTime("06/22/2017"), endDate = Convert.ToDateTime("06/27/2017"), course_Id = 14 }
            );
            context.TaskTypeModels.AddOrUpdate(x => x.taskType_Id,
                new TaskTypeModels { taskType = "Test", taskWeight = 40 },
                new TaskTypeModels { taskType = "Assignment", taskWeight = 20 },
                new TaskTypeModels { taskType = "Project", taskWeight = 40 }
            );
            context.taskModel.AddOrUpdate(x => x.task_Id,
                new TaskModel { taskType_Id = 4, taskName = "Python test", startDate = Convert.ToDateTime("06/12/2017"), endDate = Convert.ToDateTime("06/12/2017"), points = 40, unit_Id = 2, taskNotes = "a test" },
                new TaskModel { taskType_Id = 4, taskName = "Binary Search Tree", startDate = Convert.ToDateTime("06/07/2017"), endDate = Convert.ToDateTime("06/09/2017"), points = 30, unit_Id = 2, taskNotes = "" },
                new TaskModel { taskType_Id = 4, taskName = "Lemonade Stand", startDate = Convert.ToDateTime("06/09/2017"), endDate = Convert.ToDateTime("06/12/2017"), points = 30, unit_Id = 3, taskNotes = "" }
                );
            context.classmodel.AddOrUpdate(x => x.class_Id,
                new ClassModel { className = "Flourine", program_id = 1 },
                new ClassModel { className = "Oxygen", program_id = 1 },
                new ClassModel { className = "Magnesium", program_id = 1 },
                new ClassModel { className = "Gold", program_id = 1 },
                new ClassModel { className = "Plutonium", program_id = 2 },
                new ClassModel { className = "Barium", program_id = 2 }
            );
            context.Roles.AddOrUpdate(x => x.Id,
               new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "Admin"  }
            );
        }
    }
}
