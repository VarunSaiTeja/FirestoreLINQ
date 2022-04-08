using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class OrderClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        public OrderClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void ToList()
        {
            var results = students.ToList();
            var mockResults = MockData.Students.ToList();

            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void OrderBy_String()
        {
            var result = students.OrderBy(x => x.FirstName).ToList().First();
            var mockResult = MockData.Students.OrderBy(x => x.FirstName).ToList().First();

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void OrderBy_Int()
        {
            var result = students.OrderBy(x => x.Age).ToList().First();
            var mockResult = MockData.Students.OrderBy(x => x.Age).ToList().First();

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void OrderByDescending_String()
        {
            var result = students.OrderByDescending(x => x.FirstName).ToList().First();
            var mockResult = MockData.Students.OrderByDescending(x => x.FirstName).ToList().First();

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void OrderByDescending_Int()
        {
            var result = students.OrderByDescending(x => x.Age).ToList().First();
            var mockResult = MockData.Students.OrderByDescending(x => x.Age).ToList().First();

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void OrderBy_ThenBy()
        {
            var results = students.OrderBy(x => x.LastName).ThenBy(x => x.Age).ToList();
            var mockResults = MockData.Students.OrderBy(x => x.LastName).ThenBy(x => x.Age).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void OrderBy_ThenByDescending()
        {
            var results = students.OrderBy(x => x.LastName).ThenByDescending(x => x.Age).ToList();
            var mockResults = MockData.Students.OrderBy(x => x.LastName).ThenByDescending(x => x.Age).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void OrderByDescending_ThenBy()
        {
            var results = students.OrderByDescending(x => x.LastName).ThenBy(x => x.Age).ToList();
            var mockResults = MockData.Students.OrderByDescending(x => x.LastName).ThenBy(x => x.Age).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void OrderByDescending_ThenByDescending()
        {
            var results = students.OrderByDescending(x => x.LastName).ThenByDescending(x => x.Age).ToList();
            var mockResults = MockData.Students.OrderByDescending(x => x.LastName).ThenByDescending(x => x.Age).ToList();
            
            Assert.Equal(mockResults,results);
        }
    }
}
