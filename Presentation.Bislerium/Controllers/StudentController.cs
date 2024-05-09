using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;  
        }
        [HttpPost, Route("AddStudent")]

        public async Task<IActionResult> AddStudent(Student student)
        {
            var addStudent = await _studentService.AddStudent(student);
            return Ok(addStudent);
        }

        [HttpPost, Route("GetAllStudents")]
       // [Authorize]

        public async Task<IActionResult> GetAllStudents()
        {
            return Ok(await _studentService.GetAllStudents());
        }
        //[HttpPost,Route("UpdateStudents")]


        public IActionResult Index()
        {
            return View();
        }
    }
}
