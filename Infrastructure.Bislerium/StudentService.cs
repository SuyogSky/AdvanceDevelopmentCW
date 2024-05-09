using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class StudentService : IStudentService
    {
        private readonly AplicationDBContext _dbContext;
        public StudentService(AplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Student> AddStudent(Student student)
        {
            var result = await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<Student> DeleteStudent(String id)
        {
            var student = await _dbContext.Students.Where(s => s.Id.ToString() == id.ToString()).ToListAsync();
            if (student != null)
            {
                _dbContext.Students.Remove(student[0]);
                await _dbContext.SaveChangesAsync();
            }
            return student![0];
        }




        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await _dbContext.Students.ToListAsync();
        }




        public async Task<IEnumerable<Student>> GetStudentById(string id)
        {
            var result = await _dbContext.Students.Where(s => s.Id.ToString() == id.ToString()).ToListAsync();
            return result;
        }

        public async Task<Student> UpdateStudent(Student studentToUpdate)
        {
            _dbContext.Students.Update(studentToUpdate);
            await _dbContext.SaveChangesAsync();
            return studentToUpdate;
        }

        async Task<Student> IStudentService.DeleteStudent(string id)
        {
            var student = await _dbContext.Students.Where(s => s.Id.ToString() == id.ToString()).ToListAsync();
            if (student != null)
            {
                _dbContext.Students.Remove(student[0]);
                await _dbContext.SaveChangesAsync();
            }
            return student![0];
        }
    }
}
