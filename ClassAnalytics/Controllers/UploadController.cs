using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models;
using System.Web.Mvc;
using System.Data.Entity;

namespace ClassAnalytics.Controllers
{
    public class UploadController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
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
                db.Entry(upload).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("uploadList/" + upload.class_id);
            }
            return View(upload);
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
                return RedirectToAction("uploadList");
            }
        }
        public ActionResult uploadList(int? id)
        {
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
                return RedirectToAction("Index");
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
        public ActionResult Upload(HttpPostedFileBase file, UploadViewModel pdf)
        {
            UploadModel newPdf = new UploadModel();
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
            if (file == null)
            {
                pdf.classModel = db.classmodel.Find(pdf.class_id);
                pdf.courses = courses(pdf.classModel.program_id);
                ViewBag.StatusMessage = "File is invalid";
                return View(pdf);
            }
            else
            {
                var ext = file.FileName.Split('.');
                string exts = ext[ext.Count() - 1];
                if (exts == "pdf")
                {
                    string path = null;
                    if (pdf.uploadName != null)
                    {
                        pdf.uploadName = pdf.uploadName + "." + exts;
                        path = Server.MapPath("~/Assignments/" + pdf.uploadName);
                    }
                    else
                    {
                        path = Server.MapPath("~/Assignments/" + file.FileName);
                        pdf.uploadName = file.FileName;
                    }
                    file.SaveAs(path);
                    newPdf = viewToModel(pdf, path);
                    db.uploadModel.Add(newPdf);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Class");
                }
                else
                {
                    pdf.classModel = db.classmodel.Find(pdf.class_id);
                    pdf.courses = courses(pdf.classModel.program_id);
                    ViewBag.StatusMessage = "File is not in PDF format.";
                    return View(pdf);
                }
            }
        }
        public UploadModel viewToModel(UploadViewModel viewModel, string path)
        {
            UploadModel newPdf = new UploadModel();
            if(viewModel.upload_id == 0) {
                newPdf.upload_id = viewModel.upload_id;
                newPdf.filePath = viewModel.filePath;
                newPdf.createDate = viewModel.createDate;
            }
            else
            {
                newPdf.filePath = path;
                newPdf.createDate = DateTime.Now;
            }
            newPdf.active = viewModel.active;
            newPdf.class_id = viewModel.class_id;
            newPdf.course_Id = viewModel.course_Id;
            newPdf.uploadName = viewModel.uploadName;
            newPdf.uploadType = viewModel.uploadType;
            return newPdf;
        }
        public UploadViewModel modelToView(UploadModel upload)
        {
            UploadViewModel newPdf = new UploadViewModel();
            newPdf.filePath = upload.filePath;
            newPdf.createDate = upload.createDate;
            newPdf.active = upload.active;
            newPdf.class_id = upload.class_id;
            newPdf.course_Id = upload.course_Id;
            newPdf.uploadName = upload.uploadName;
            newPdf.uploadType = upload.uploadType;
            return newPdf;
        }
        public List<SelectListItem> courses(int program_id)
        {
            List<CourseModels> courses = db.coursemodels.ToList();
            List<SelectListItem> new_courses = new List<SelectListItem>();
            new_courses.Add(new SelectListItem() { Text ="Select Course [Optional]", Value=null });
            foreach (CourseModels course in courses)
            {
                if (course.program_Id == program_id)
                {
                    new_courses.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                }
            }
            return new_courses;
        }
    }
}