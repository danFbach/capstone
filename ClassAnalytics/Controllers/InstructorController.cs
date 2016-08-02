using System;
using System.IO;
using System.Web;
using System.Net;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Instructor_Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassAnalytics.Controllers
{
    public class InstructorController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public void AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public List<ClassModel> getClassList(int instructor_id)
        {
            List<InstrctrClassJoin> instPrograms = db.instructorClassJoin.ToList();
            List<ClassModel> classes = new List<ClassModel>();
            foreach(InstrctrClassJoin _classJoin in instPrograms)
            {
                if(_classJoin.instructor_id == instructor_id)
                {
                    ClassModel _class = db.classmodel.Find(_classJoin.class_id);
                    _class.ProgramModels = db.programModels.Find(_class.program_id);
                    classes.Add(_class);
                }
            }
            return classes;
        }
        public List<SelectListItem> getClassSelectList(InstructorModel instructor)
        {
            List<ClassModel> classes = new List<ClassModel>();
            List<SelectListItem> newClasses = new List<SelectListItem>();
            List<InstrctrClassJoin> instClasses = db.instructorClassJoin.ToList();
            List<ClassModel> classes1 = db.classmodel.ToList();
            foreach (ClassModel this_class in classes1)
            {
                foreach (InstrctrClassJoin a_class in instClasses)
                {
                    a_class.classModel = db.classmodel.Find(a_class.class_id);
                    if (a_class.instructor_id == instructor.instructor_Id && a_class.class_id == this_class.class_Id)
                    {
                        break;
                    }
                }
                newClasses.Add(new SelectListItem() { Text = this_class.className, Value = this_class.class_Id.ToString() });
                classes.Add(this_class);
            }
            return newClasses;
        }
        public ActionResult addToClass(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                InstructorClassJoinModel joinModel = new InstructorClassJoinModel();
                InstructorModel instructor = db.instructorModel.Find(id);
                joinModel.instructor = instructor;
                joinModel.instructor_Id = Convert.ToInt16(id);
                joinModel.classes = getClassSelectList(instructor);
                return View(joinModel);
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult addToClass(InstructorClassJoinModel joinModel)
        {
            if (ModelState.IsValid)
            {
                InstrctrClassJoin newProgram = new InstrctrClassJoin();
                newProgram.instructor_id = joinModel.instructor_Id;
                newProgram.class_id = joinModel.class_id;
                db.instructorClassJoin.Add(newProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            joinModel.instructor = db.instructorModel.Find(joinModel.instructor_Id);
            getClassSelectList(joinModel.instructor);
            return View(joinModel);
        }
        // GET: Instructor
        public ActionResult Index()
        {
            return View(db.instructorModel.ToList());
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstructorDetailViewModel viewModel = new InstructorDetailViewModel();
            viewModel.instructor = db.instructorModel.Find(id);
            if (viewModel.instructor == null)
            {
                return HttpNotFound();
            }
            viewModel.instructor_id = viewModel.instructor.instructor_Id;
            viewModel.classList = getClassList(viewModel.instructor_id);
            return View(viewModel);
        }
        public ActionResult removeFromClass(int? class_id, int? instructor_id)
        {
            if(class_id == null || instructor_id == null)
            {
                return RedirectToAction("Details/" + instructor_id);
            }
            List<InstrctrClassJoin> classes = db.instructorClassJoin.ToList();
            InstructorClassJoinModel joinModel = new InstructorClassJoinModel();
            foreach(InstrctrClassJoin join in classes)
            {
                if(join.instructor_id == instructor_id && join.class_id == class_id)
                {
                    joinModel.class_id = join.class_id;
                    joinModel.classModel = db.classmodel.Find(join.class_id);
                    joinModel.join_id = join.id;
                    InstructorModel instructor = db.instructorModel.Find(join.instructor_id);
                    ViewBag.instructor = instructor.fName + " " + instructor.lName;
                    return View(joinModel);
                }
            }
            return RedirectToAction("Details/" + instructor_id);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult removeFromClass(InstructorClassJoinModel join)
        {
            InstrctrClassJoin joinModel = db.instructorClassJoin.Find(join.join_id);
            db.instructorClassJoin.Remove(joinModel);
            db.SaveChanges();
            return RedirectToAction("Details/" + join.instructor_Id);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InstructorModel instructorModel)
        {
            if (ModelState.IsValid)
            {
                RegisterViewModel newUser = new RegisterViewModel();
                newUser.Email = instructorModel.email;
                newUser.Password = "R3$et_this";
                newUser.ConfirmPassword = newUser.Password;
                string username = makeUserName(instructorModel.fName, instructorModel.lName, 0);
                var user = new ApplicationUser { UserName = username, Email = newUser.Email };
                var result = await UserManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    Directory.CreateDirectory(Server.MapPath("~//Uploads//instructorData//" + user.Id));
                    instructorModel.instructor_account_Id = user.Id;
                    addRole(user);
                    db.instructorModel.Add(instructorModel);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(instructorModel);
        }
        public void addRole(ApplicationUser user)
        {
            IdentityUserRole role = new IdentityUserRole();
            role.UserId = user.Id;
            role.RoleId = "New User";
            UserManager.AddToRole(role.UserId, role.RoleId);
        }
        public string makeUserName(string fName,string lName,int count)
        {
            string username;
            if (count == 0)
            {
                username = fName.ToLower() + lName;
            }
            else
            {
                username = fName.ToLower() + count.ToString() + lName;
            }
            ApplicationUser this_user = UserManager.FindByName(username);
            if (this_user != null)
            {
                count += 1;
                return makeUserName(fName, lName, count);
            }
            else
            {
                return username;
            }
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstructorModel instructorModel = db.instructorModel.Find(id);
            if (instructorModel == null)
            {
                return HttpNotFound();
            }
            return View(instructorModel);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InstructorModel instructorModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(instructorModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(instructorModel);
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstructorModel instructorModel = db.instructorModel.Find(id);
            if (instructorModel == null)
            {
                return HttpNotFound();
            }
            return View(instructorModel);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InstructorModel instructorModel = db.instructorModel.Find(id);
            db.instructorModel.Remove(instructorModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
