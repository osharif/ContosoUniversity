using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        //private SchoolContext db = new SchoolContext();

        private IStudentRepository studentRepository;

        public StudentsController()
        {
           this.studentRepository = new StudentRepository(new SchoolContext());
        }

        public StudentsController(IStudentRepository studentRepository)
        {
             this.studentRepository = studentRepository;
        }

        // GET: Students
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentOrder = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "NameDsc" : "";
            ViewBag.FirstMidNameSortParam = sortOrder == "FirstMidNameAsc" ? "FirstMidNameDsc" : "FirstMidNameAsc";
            ViewBag.DateSortParam = sortOrder == "Date" ? "DateDsc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var students = from s in studentRepository.GetStudents()
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                    || s.FirstMidName.ToUpper().Contains(searchString.ToUpper()));
            }


            switch (sortOrder)
            {
                case "NameDsc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "FirstMidNameAsc":
                    students = students.OrderBy(s => s.FirstMidName);
                    break;
                case "FirstMidNameDsc":
                    students = students.OrderByDescending(s => s.FirstMidName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "DateDsc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default: // Name ascending
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Students/Details/5        
        public ViewResult Details(int id)
        {

            Student student = studentRepository.GetStudentByID(id);
            return View(student);
        }

        // GET: Students/Create       
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ID,LastName,FirstMidName,EnrollmentDate")] 
            Student student,
            HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        student.Files = new List<File> { avatar };
                    }

                    studentRepository.InsertStudent(student);
                    studentRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /*dex*/)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {

            Student student = studentRepository.GetStudentByID(id);
            
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ID,LastName,FirstMidName,EnrollmentDate")]
            Student student,
            HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (student.Files.Any(f => f.FileType == FileType.Avatar))
                        {
                            student.Files.Remove(student.Files.First(f => f.FileType == FileType.Avatar));
                        }
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        student.Files = new List<File> { avatar };
                    }
                    studentRepository.UpdateStudent(student);
                    studentRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch(DataException /* dex*/)
            {
                //log the error, uncomment here
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = studentRepository.GetStudentByID(id);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                /*      old version deleting operation
                Student student = db.Students.Find(id);
                db.Students.Remove(student); 
                 * */

                
                Student student = studentRepository.GetStudentByID(id);
                studentRepository.DeleteStudent(id);
                studentRepository.Save();

            }
            catch (DataException /*dex*/)
            {
                //uncomment dex and log error
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                studentRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
