using System.Collections.Generic;
using s17738_cw3.Models;

namespace s17738_cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
    }
}
