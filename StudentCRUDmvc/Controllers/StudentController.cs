using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCRUDmvc.Models;
using System.Data.Entity;
using System.IO;

namespace StudentCRUDmvc.Controllers
{
    public class StudentController : Controller
    {
        NurEntities db = new NurEntities();

        // GET: Student
        public ActionResult Index()
        {
            var stu = db.Students
                        .Include(a => a.Admissions.Select(s => s.Subject))
                        .OrderByDescending(si => si.Stu_id)
                        .ToList();
            return View(stu);
        }

        // GET: Student/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Subjects = db.Subjects.ToList();
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        public ActionResult Create(Student Studentvm, int[] sub_id)
        {
            if (ModelState.IsValid)
            {
                var student = new Student()
                {
                    Stu_name = Studentvm.Stu_name,
                    Age = Studentvm.Age,
                    DateOfBirth = Studentvm.DateOfBirth,
                    MorningShift = Studentvm.MorningShift
                };

                HttpPostedFileBase file = Studentvm.PictureFile;
                if (file != null)
                {
                    string filename = Path.Combine("/Images/", file.FileName);
                    file.SaveAs(Server.MapPath(filename));
                    student.Picture = filename;
                }

                foreach (var s_id in sub_id)
                {
                    var id = db.Subjects.FirstOrDefault(a => a.Sub_id == s_id);
                    if (id != null)
                    {
                        var admission = new Admission()
                        {
                            Student = student,
                            Sub_id = id.Sub_id
                        };
                        db.Admissions.Add(admission);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Subjects = db.Subjects.ToList();
            return View(Studentvm);
        }

        // GET: Student/Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var student = db.Students.Include(s => s.Admissions)
                                     .FirstOrDefault(s => s.Stu_id == id);

            if (student == null)
                return HttpNotFound();

            ViewBag.Subjects = db.Subjects.ToList();
            ViewBag.SelectedSubjects = student.Admissions.Select(a => a.Sub_id).ToArray();

            return View(student);
        }

        // POST: Student/Edit
        [HttpPost]
        public ActionResult Edit(Student studentVm, int[] sub_id)
        {
            if (ModelState.IsValid)
            {
                var student = db.Students.Include(s => s.Admissions)
                                         .FirstOrDefault(s => s.Stu_id == studentVm.Stu_id);
                if (student == null)
                    return HttpNotFound();

                student.Stu_name = studentVm.Stu_name;
                student.Age = studentVm.Age;
                student.DateOfBirth = studentVm.DateOfBirth;
                student.MorningShift = studentVm.MorningShift;

                HttpPostedFileBase file = studentVm.PictureFile;
                if (file != null)
                {
                    string filename = Path.Combine("/Images/", file.FileName);
                    file.SaveAs(Server.MapPath(filename));
                    student.Picture = filename;
                }

                // Remove existing subjects
                db.Admissions.RemoveRange(student.Admissions);

                // Add new selected subjects
                foreach (var sid in sub_id)
                {
                    db.Admissions.Add(new Admission
                    {
                        Stu_id = student.Stu_id,
                        Sub_id = sid
                    });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Subjects = db.Subjects.ToList();
            ViewBag.SelectedSubjects = sub_id;
            return View(studentVm);
        }

        // GET: Student/Delete
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var student = db.Students.Include(s => s.Admissions)
                                     .FirstOrDefault(s => s.Stu_id == id);
            if (student == null)
                return HttpNotFound();

            return View(student);
        }

        // POST: Student/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var student = db.Students.Include(s => s.Admissions)
                                     .FirstOrDefault(s => s.Stu_id == id);
            if (student == null)
                return HttpNotFound();

            db.Admissions.RemoveRange(student.Admissions);
            db.Students.Remove(student);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
