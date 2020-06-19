using System.Collections.Generic;
using System.Threading.Tasks;
using s17738_cw3.DTO;
using s17738_cw3.OrmModels;

namespace s17738_cw3.DAL
{
    public interface IDbService
    {
        IEnumerable<Student> GetStudents();

        Task<Student> GetStudent(string indexNumber);

        Task<Student> CreateStudent(Student student);

        Task<bool> UpdateStudent(string indexNumber, Student student);

        Task<bool> DeleteStudent(string indexNumber);

        Task<Enrollment> GetStudentEnrollments(string indexNumber);

        bool EnrollmentExist(int semester, string studies);

        Enrollment EnrollStudent(EnrollStudentRequest enrollStudentRequest);

        Enrollment PromoteStudents(int semester, string studies);

        Token GetToken(string token);

        void SaveToken(Token authToken);
    }
}
