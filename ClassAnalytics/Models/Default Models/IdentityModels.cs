using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassAnalytics.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Class_Models.ClassModel> classmodel { get; set; }
        public DbSet<Misc_Models.CourseModels> coursemodels { get; set; }
        public DbSet<Gradebook_Models.GradeBookModel> gradeBookModel { get; set; }
        public DbSet<Program_Models.ProgramModels> programModels { get; set; }
        public DbSet<Class_Models.StudentModels> studentModels { get; set; }
        public DbSet<Survey_Models.SurveyAnswers> surveyAnswers { get; set; }
        public DbSet<Survey_Models.SurveyJoinTableModel> surveyJoinTableModel { get; set; }
        public DbSet<Survey_Models.SurveyModel> surveyModel { get; set; }
        public DbSet<Survey_Models.SurveyQuestion> surveyQuestion { get; set; }
        public DbSet<Task_Models.TaskModel> taskModel { get; set; }
        public DbSet<Misc_Models.InstructorModel> instructorModel { get; set; }
        public DbSet<Misc_Models.MessagingModel> messagingModel { get; set; }
        public DbSet<Uploads_Models.UploadModel> uploadModel { get; set; }
        public DbSet<Uploads_Models.studentUploads> studentUpload { get; set; }
        public System.Data.Entity.DbSet<ClassAnalytics.Models.Task_Models.TaskTypeModels> TaskTypeModels { get; set; }
        
    }
}