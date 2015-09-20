using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        private GenericRepository<File> fileRepository;

        public FileController()
        {
            this.fileRepository = new GenericRepository<File>(new SchoolContext());
        }
            

        public ActionResult Index(int id)
        {
            var fileToRetrieve = fileRepository.GetByID(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}