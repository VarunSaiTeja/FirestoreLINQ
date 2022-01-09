using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class OrderClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.Collection("Students").AsQuerable<Student>();

        public OrderClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void ToList()
        {
            var results = students.ToList();
            Assert.Equal(3, results.Count);
        }

        [Fact]
        public void OrderBy_String()
        {
            var result = students.OrderBy(x => x.FirstName).ToList().First();
            Assert.Equal("Harshini", result.FirstName);
        }

        [Fact]
        public void OrderBy_Int()
        {
            var result = students.OrderBy(x => x.Age).ToList().First();
            Assert.Equal("Harshini", result.FirstName);
        }

        [Fact]
        public void OrderByDescending_String()
        {
            var result = students.OrderByDescending(x => x.FirstName).ToList().First();
            Assert.Equal("Varun", result.FirstName);
        }

        [Fact]
        public void OrderByDescending_Int()
        {
            var result = students.OrderByDescending(x => x.Age).ToList().First();
            Assert.Equal("Varun", result.FirstName);
        }

        [Fact]
        public void OrderBy_ThenBy()
        {
            var results = students.OrderBy(x => x.LastName).ThenBy(x => x.Age).ToList();

            Assert.Equal(3, results.Count);

            Assert.Equal("Harshini", results[0].FirstName);
            Assert.Equal("Sai", results[1].FirstName);
            Assert.Equal("Varun", results[2].FirstName);
        }

        [Fact]
        public void OrderBy_ThenByDescending()
        {
            var results = students.OrderBy(x => x.LastName).ThenByDescending(x => x.Age).ToList();

            Assert.Equal(3, results.Count);

            Assert.Equal("Harshini", results[0].FirstName);
            Assert.Equal("Varun", results[1].FirstName);
            Assert.Equal("Sai", results[2].FirstName);
        }

        [Fact]
        public void OrderByDescending_ThenBy()
        {
            var results = students.OrderByDescending(x => x.LastName).ThenBy(x => x.Age).ToList();

            Assert.Equal(3, results.Count);

            Assert.Equal("Sai", results[0].FirstName);
            Assert.Equal("Varun", results[1].FirstName);
            Assert.Equal("Harshini", results[2].FirstName);
        }

        [Fact]
        public void OrderByDescending_ThenByDescending()
        {
            var results = students.OrderByDescending(x => x.LastName).ThenByDescending(x => x.Age).ToList();

            Assert.Equal(3, results.Count);

            Assert.Equal("Varun", results[0].FirstName);
            Assert.Equal("Sai", results[1].FirstName);
            Assert.Equal("Harshini", results[2].FirstName);
        }
    }
}
