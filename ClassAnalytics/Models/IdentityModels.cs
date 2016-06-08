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
        public DbSet<ClassModel> classmodel { get; set; }
        public DbSet<CourseModels> coursemodels { get; set; }
        public DbSet<ProgramModels> programModels { get; set; }
        public DbSet<StudentModels> studentModels { get; set; }
        public DbSet<TaskModel> taskModel { get; set; }
        public DbSet<UnitModels> unitModels { get; set; }
        public DbSet<GradeBookModel> gradeBookModel { get; set; }

        public System.Data.Entity.DbSet<ClassAnalytics.Models.TaskTypeModels> TaskTypeModels { get; set; }
    }
}