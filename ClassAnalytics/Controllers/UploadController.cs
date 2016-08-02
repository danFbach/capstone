using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Uploads_Models;
using Microsoft.AspNet.Identity;

namespace ClassAnalytics.Controllers
{
    public class UploadController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult studentUpload(int? id)
        {
            studentUploads uploads = new studentUploads();
            if (id != null)
            {
                uploads.task_id = id;
            }
            return View(uploads);
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult studentUpload(HttpPostedFileBase file, studentUploads newUpload)
        {
            string userID = User.Identity.GetUserId();
            string className = "";
            List<StudentModels> students = db.studentModels.ToList();
            StudentModels this_student = new StudentModels();
            foreach(StudentModels student in students)
            {
                if(student.student_account_Id == userID)
                {
                    this_student = student;
                }
            }
            this_student.ClassModel = db.classmodel.Find(this_student.class_Id);
            className = this_student.ClassModel.className;
            if(file == null)
            {
                ViewBag.StatusMessage = "No file Selected";
                return View(newUpload);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var strs = file.FileName.Split('.');
                    string ext = strs[strs.Count() - 1];
                    if(ext == "pdf" || ext == "txt" || ext == "css" || ext == "html" || ext == "js")
                    {
                        if(newUpload.file_name == null)
                        {
                            newUpload.file_name = file.FileName;
                        }
                        else
                        {
                            newUpload.file_name += ("." + ext);
                        }
                        string path = Server.MapPath("~//Uploads//classData//" + className + "//" + userID + "//" + newUpload.file_name);
                        file.SaveAs(path);
                        newUpload.createDate = DateTime.Now;
                        newUpload.student_id = this_student.student_Id;
                        db.studentUpload.Add(newUpload);
                        db.SaveChanges();
                        return RedirectToAction("studentIndex");
                    }
                    else
                    {
                        return View(newUpload);
                    }
                }
                else
                {
                    ViewBag.StatusMessage = "Invalid Filetype.";
                    return View(newUpload);
                }
            }
        }
        public ActionResult studentIndex()
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<StudentModels> students = db.studentModels.ToList();
            List<studentUploads> uploads = db.studentUpload.ToList();
            List<UploadModel> classUploads = db.uploadModel.ToList();
            StudentModels thisStudent = new StudentModels();
            studentUploadViewModel viewModel = new studentUploadViewModel();
            viewModel.uploadList = new List<UploadModel>();
            viewModel.studentUploadList = new List<studentUploads>();
            foreach(StudentModels student in students)
            {
                if(student.student_account_Id == UserId)
                {
                    thisStudent = student;
                    break;
                }
            }
            ViewBag.student = thisStudent.fName;
            foreach(studentUploads upload in uploads)
            {
                if(upload.student_id == thisStudent.student_Id)
                {
                    viewModel.studentUploadList.Add(upload);
                }
            }
            foreach(UploadModel upload in classUploads)
            {
                if(upload.class_id == thisStudent.class_Id)
                {
                    upload.courseModels = db.coursemodels.Find(upload.course_Id);
                    viewModel.uploadList.Add(upload);
                }
            }
            viewModel.uploadList = viewModel.uploadList.OrderByDescending(x => x.createDate).ToList();
            return View(viewModel);
        }
        public ActionResult Edit(int? id)
        {
            if(id != null)
            {
                UploadModel upload = db.uploadModel.Find(id);
                List<CourseModels> courses = db.coursemodels.ToList();
                ClassModel _class = db.classmodel.Find(upload.class_id);
                UploadViewModel viewModel = new UploadViewModel();
                viewModel = modelToView(upload);
                viewModel.courses = new List<SelectListItem>();
                foreach(CourseModels course in courses)
                {
                    if(course.program_Id == _class.program_id)
                    {
                        viewModel.courses.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                    }
                }
                return View(viewModel);
            }
            return RedirectToAction("Index", "Class");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(UploadViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                UploadModel upload = new UploadModel();
                upload = viewToModel(ViewModel);
                UploadModel oldFile = db.uploadModel.Find(upload.upload_id);
                var strs = upload.uploadName.Split('.');
                string ext = strs[strs.Count()-1];
                var oldstr = oldFile.uploadName.Split('.');
                string oldext = oldstr[oldstr.Count() - 1];
                upload.classModel = db.classmodel.Find(upload.class_id);
                string newRelative = "//Uploads//classData//" + upload.classModel.className + "//instructorUploads//" + upload.uploadType + "//";
                if (ext != oldext)
                {
                    upload.uploadName += ( "." + oldext);
                }
                if (System.IO.File.Exists(Server.MapPath(oldFile.relativePath)))
                {
                    System.IO.File.Move(Server.MapPath(oldFile.relativePath), Server.MapPath(newRelative + upload.uploadName));
                }
                else
                {
                    return RedirectToAction("Index", "Class");
                }
                db.Entry(oldFile).CurrentValues.SetValues(upload);
                oldFile.relativePath = newRelative + upload.uploadName;
                db.Entry(oldFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Class");
            }
            return View(ViewModel);
        }
        public ActionResult ViewFile(int? id)
        {
            if(id != null)
            {
                UploadModel upload = db.uploadModel.Find(id);
                var strs = upload.uploadName.Split('.');
                string exts = strs[strs.Count() - 1];
                if(exts == "pdf")
                {
                    return RedirectToAction("PDFView/" + id);
                }
                else if(exts == "txt" || exts == "css" || exts == "js" || exts == "html")
                {
                    return RedirectToAction("textView/" + id);
                }
                else
                {
                    return RedirectToAction("Index", "Class");
                }
            }
            else
            {
                return RedirectToAction("Index", "Class");
            }
        }
        public ActionResult textView(int? id)
        {
            if (id != null)
            {
                UploadModel upload = db.uploadModel.Find(id);
                return View(upload);
            }
            else
            {
                return RedirectToAction("Index", "Class");
            }
        }
        public ActionResult PDFView(int? id)
        {
            if (id != null)
            {
                UploadModel upload = db.uploadModel.Find(id);
                return View(upload);
            }
            else
            {
                return RedirectToAction("Index", "Class");
            }
        }
        public ActionResult uploadPartial(int? id)
        {
            if(id != null)
            {
                UploadViewModel viewModel = new Models.Uploads_Models.UploadViewModel();
                viewModel.class_id = id;
                ClassModel _class = db.classmodel.Find(id);
                viewModel.courses = courses(_class.program_id);                
                return PartialView(viewModel);
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult uploadPartial(HttpPostedFileBase file, UploadViewModel uploadModel)
        {
            UploadModel upload = new UploadModel();
            if(file == null)
            {
                return RedirectToAction("Index", "Class");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    uploadModel.classModel = db.classmodel.Find(uploadModel.class_id);
                    string relativePath = "";
                    var strs = file.FileName.Split('.');
                    string exts = strs[strs.Count() - 1];
                    relativePath = "//Uploads//classData//" + uploadModel.classModel.className + "//instructorUploads//" + uploadModel.uploadType + "//";
                    string path = null;
                    if (!Directory.Exists(Server.MapPath(relativePath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(relativePath));
                    }
                    if (uploadModel.uploadName != null)
                    {
                        uploadModel.uploadName = uploadModel.uploadName + "." + exts;
                        path = Server.MapPath(relativePath + uploadModel.uploadName);
                    }
                    else
                    {
                        path = Server.MapPath(relativePath + file.FileName);
                        uploadModel.uploadName = file.FileName;
                    }
                    uploadModel.relativePath = relativePath + uploadModel.uploadName;
                    upload = viewToModel(uploadModel);
                    db.uploadModel.Add(upload);
                    db.SaveChanges();
                    file.SaveAs(path);
                    return RedirectToAction("Index", "Class");
                }
                else
                {
                    return RedirectToAction("Index", "Class");
                }
            }
        }
        public ActionResult uploadList(int? id)
        {
            if (this.User.IsInRole("Student"))
            {
                return RedirectToAction("studentIndex");
            }
            if (id != null)
            {
                uploadListViewModel viewModel = new uploadListViewModel();
                viewModel.instructorUploads = new List<UploadModel>();
                viewModel.studentUploads = new List<studentUploads>();
                List<UploadModel> uploads = db.uploadModel.ToList();
                List<studentUploads> stUploads = db.studentUpload.ToList();
                foreach (UploadModel upload in uploads)
                {
                    if (upload.class_id == id)
                    {
                        upload.classModel = db.classmodel.Find(upload.class_id);
                        if(upload.course_Id != null)
                        {
                            upload.courseModels = db.coursemodels.Find(upload.course_Id);
                        }
                        else
                        {
                            upload.courseModels = new CourseModels();
                            upload.courseModels.courseName = "No Course";
                        }
                        viewModel.instructorUploads.Add(upload);
                    }
                }
                foreach(studentUploads upload in stUploads)
                {
                    upload.studentModel = db.studentModels.Find(upload.student_id);
                    if(upload.studentModel.class_Id == id)
                    {
                        viewModel.studentUploads.Add(upload);
                    }
                }
                ViewBag.className = db.classmodel.Find(id).className;
                viewModel.instructorUploads.OrderBy(m => m.createDate);
                viewModel.studentUploads.OrderBy(m => m.createDate);
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index","Class");
            }
        }

        public ActionResult Upload(int? id)
        {
            if (id != null)
            {
                UploadViewModel upload = new UploadViewModel();
                upload.class_id = id;
                upload.classModel = db.classmodel.Find(id);
                upload.courses = courses(upload.classModel.program_id);
                return View(upload);
            }
            else
            {
                return RedirectToAction("Index", "Class");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file, UploadViewModel uploadModel)
        {
            UploadModel newUpload = new UploadModel();
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
            if (file == null)
            {
                uploadModel.classModel = db.classmodel.Find(uploadModel.class_id);
                uploadModel.courses = courses(uploadModel.classModel.program_id);
                ViewBag.StatusMessage = "File is invalid";
                return View(uploadModel);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    uploadModel.classModel = db.classmodel.Find(uploadModel.class_id);
                    string relativePath = "";
                    var strs = file.FileName.Split('.');
                    string exts = strs[strs.Count() - 1];
                    if (exts == "pdf" || exts == "txt" || exts == "css" || exts == "js" || exts == "html")
                    {
                        relativePath = "~//Uploads/classData//" + uploadModel.classModel.className + "//instructorUploads//" + uploadModel.uploadType + "//";
                        string path = null;
                        if (!Directory.Exists(Server.MapPath(relativePath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(relativePath));
                        }
                        if (uploadModel.uploadName != null)
                        {
                            uploadModel.uploadName = uploadModel.uploadName + "." + exts;
                            path = Server.MapPath(relativePath + uploadModel.uploadName);
                        }
                        else
                        {
                            path = Server.MapPath(relativePath + file.FileName);
                            uploadModel.uploadName = file.FileName;
                        }
                        uploadModel.relativePath = "classData//" + uploadModel.classModel.className + "//instructorUploads//" + uploadModel.uploadType + "//";
                        newUpload = viewToModel(uploadModel);
                        db.uploadModel.Add(newUpload);
                        db.SaveChanges();
                        file.SaveAs(path);
                        return RedirectToAction("uploadList/" + uploadModel.class_id );
                    }
                    else
                    {
                        uploadModel.classModel = db.classmodel.Find(uploadModel.class_id);
                        uploadModel.courses = courses(uploadModel.classModel.program_id);
                        return View(uploadModel);
                    }
                }
                else
                {
                    uploadModel.classModel = db.classmodel.Find(uploadModel.class_id);
                    uploadModel.courses = courses(uploadModel.classModel.program_id);
                    ViewBag.StatusMessage = "File is not in pdf, css, js, html or txt format.";
                    return View(uploadModel);
                }
            }
        }
        public UploadModel viewToModel(UploadViewModel viewModel)
        {
            UploadModel newUpload = new UploadModel();
            if(viewModel.upload_id != 0) {
                newUpload.upload_id = viewModel.upload_id;
                newUpload.relativePath = viewModel.relativePath;
                newUpload.createDate = viewModel.createDate;
            }
            else
            {
                newUpload.relativePath = viewModel.relativePath;
                newUpload.createDate = DateTime.Now;
            }
            newUpload.active = viewModel.active;
            newUpload.class_id = viewModel.class_id;
            newUpload.course_Id = viewModel.course_Id;
            newUpload.uploadName = viewModel.uploadName;
            newUpload.uploadType = viewModel.uploadType;
            return newUpload;
        }
        public UploadViewModel modelToView(UploadModel upload)
        {
            UploadViewModel newUpload = new UploadViewModel();
            if (upload.upload_id != 0)
            {
                newUpload.upload_id = upload.upload_id;
            }
            newUpload.relativePath = upload.relativePath;
            newUpload.createDate = upload.createDate;
            newUpload.active = upload.active;
            newUpload.class_id = upload.class_id;
            newUpload.course_Id = upload.course_Id;
            newUpload.uploadName = upload.uploadName;
            newUpload.uploadType = upload.uploadType;
            return newUpload;
        }
        public List<SelectListItem> courses(int program_id)
        {
            List<CourseModels> courses = db.coursemodels.ToList();
            List<SelectListItem> new_courses = new List<SelectListItem>();
            foreach (CourseModels course in courses)
            {
                if (course.program_Id == program_id)
                {
                    new_courses.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                }
            }
            return new_courses;
        }
        public ActionResult Delete(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UploadModel upload = db.uploadModel.Find(id);
            upload.courseModels = db.coursemodels.Find(upload.course_Id);
            upload.classModel = db.classmodel.Find(upload.class_id);
            if (upload == null)
            {
                return HttpNotFound();
            }
            return View(upload);
        }

        // POST: Survey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) //id is upload_id
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            UploadModel upload = db.uploadModel.Find(id);
            if (System.IO.File.Exists(upload.relativePath))
            {
                System.IO.File.Delete(upload.relativePath);
            }
            db.uploadModel.Remove(upload);
            db.SaveChanges();
            return RedirectToAction("uploadList", upload.class_id);
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