using System.Collections.Generic;
using s17738_cw3.DTO;
using s17738_cw3.Models;

namespace s17738_cw3.DAL
{
    public interface IDbService
    {
        IEnumerable<Student> GetStudents();

        Student GetStudent(string indexNumber);

        IEnumerable<Enrollment> GetStudentEnrollments(string indexNumber);

        bool EnrollmentExist(int semester, string studies);

        Enrollment EnrollStudent(EnrollStudentRequest enrollStudentRequest);

        Enrollment PromoteStudents(int semester, string studies);
    }
}
