using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity;

namespace ClassAnalytics.Controllers
{
    public class UploadController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult studentIndex()
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<StudentModels> students = db.studentModels.ToList();
            List<UploadModel> uploads = db.uploadModel.ToList();
            List<UploadModel> thisUploads = new List<UploadModel>();
            StudentModels thisStudent = new StudentModels();
            foreach(StudentModels student in students)
            {
                if(student.student_account_Id == UserId)
                {
                    thisStudent = student;
                    break;
                }
            }
            ViewBag.student = thisStudent.fName;
            foreach(UploadModel upload in uploads)
            {
                if(upload.class_id == thisStudent.class_Id)
                {
                    if(upload.active == true)
                    {
                        thisUploads.Add(upload);
                    }
                }
            }
            return View(thisUploads);
        }
        public ActionResult Edit(int? id)
        {
            if(id != null)
            {
                UploadModel upload = db.uploadModel.Find(id);
                return View(upload);
            }
            return RedirectToAction("Index", "Class");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(UploadModel upload)
        {
            if (ModelState.IsValid)
            {
                UploadModel oldFile = db.uploadModel.Find(upload.upload_id);
                var strs = upload.uploadName.Split('.');
                string ext = strs[strs.Count()-1];
                var oldstr = oldFile.uploadName.Split('.');
                string oldext = oldstr[oldstr.Count() - 1];
                string relativePath = "~/Uploads/" + upload.uploadType + "/";
                string oldRelativePath = "~/Uploads/" + oldFile.uploadType + "/";
                if(ext != oldext)
                {
                    upload.uploadName += ( "." + oldext);
                }
                if (System.IO.File.Exists(upload.filePath))
                {
                    System.IO.File.Move(Server.MapPath(oldRelativePath + oldFile.uploadName), Server.MapPath(relativePath + upload.uploadName));
                }
                UploadModel dbUpload = db.uploadModel.Find(upload.upload_id);
                if (dbUpload == null)
                {
                    db.uploadModel.Add(upload);
                    return RedirectToAction("uploadList/" + upload.class_id);
                }
                db.Entry(dbUpload).CurrentValues.SetValues(upload);
                dbUpload.filePath = Server.MapPath(relativePath+upload.uploadName);
                db.Entry(dbUpload).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("uploadList/" + upload.class_id);
            }
            return View(upload);
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
        public ActionResult uploadList(int? id)
        {
            if (this.User.IsInRole("Student"))
            {
                return RedirectToAction("studentIndex");
            }
            if (id != null)
            {
                List<UploadModel> new_uploads = new List<UploadModel>();
                List<UploadModel> uploads = db.uploadModel.ToList();
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
                            upload.courseModels.courseName = "No Course Selected";
                        }
                        new_uploads.Add(upload);
                    }
                }
                if (new_uploads.Count() == 0)
                {
                    return RedirectToAction("Index", "Class");
                }
                else
                {
                    ViewBag.className = db.classmodel.Find(id).className;
                    new_uploads.OrderBy(m => m.createDate);
                    return View(new_uploads);
                }
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
                    string relativePath = "";
                    var strs = file.FileName.Split('.');
                    string exts = strs[strs.Count() - 1];
                    if (exts == "pdf" || exts == "txt" || exts == "css" || exts == "js" || exts == "html")
                    {
                        relativePath = "~/Uploads/" + uploadModel.uploadType + "/";
                        string path = null;
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
                        newUpload = viewToModel(uploadModel, path);
                        db.uploadModel.Add(newUpload);
                        db.SaveChanges();
                        file.SaveAs(path);
                        return RedirectToAction("Index", "Class");
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
        public UploadModel viewToModel(UploadViewModel viewModel, string path)
        {
            UploadModel newUpload = new UploadModel();
            if(viewModel.upload_id != 0) {
                newUpload.upload_id = viewModel.upload_id;
                newUpload.filePath = viewModel.filePath;
                newUpload.createDate = viewModel.createDate;
            }
            else
            {
                newUpload.filePath = path;
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
            newUpload.filePath = upload.filePath;
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
            if (System.IO.File.Exists(upload.filePath))
            {
                System.IO.File.Delete(upload.filePath);
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