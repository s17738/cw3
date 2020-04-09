using System.Collections.Generic;
using s17738_cw3.Models;

namespace s17738_cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        public MockDbService()
        {
            _students = new List<Student>
            {
                new Student{ FirstName="Jan", LastName="Nowak", IndexNumber="s6251" },
                new Student{ FirstName="Adam", LastName="Kowalski", IndexNumber="s2735" },
                new Student{ FirstName="Ivan", LastName="Piotrowicz", IndexNumber="s15243" }
            };
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
