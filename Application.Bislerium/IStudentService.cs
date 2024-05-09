using Domain.Bislerium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IStudentService
    {
        Task<Student> AddStudent(Student student);

        Task<Student> UpdateStudent(Student student);
        Task<Student> DeleteStudent(string id);

        Task<IEnumerable<Student>> GetAllStudents();

        Task<IEnumerable<Student>> GetStudentById(string id);

    }
}
