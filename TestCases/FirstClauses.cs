using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class FirstClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        public FirstClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void First()
        {
            var results = students.First();
            var mockResult = MockData.Students.First();

            Assert.Equal(mockResult, results);
        }

        [Fact]
        public void OrderBy_First()
        {
            var result = students.OrderBy(x => x.FirstName).First();
            var mockResult = MockData.Students.OrderBy(x => x.FirstName).First();

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void First_InsidePredicate_Valid()
        {
            var result = students.First(x => x.Age > 15);
            var mockResult = MockData.Students.First(x => x.Age > 15);

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void First_InsidePredicate_InValid()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.First(x => x.Age > 100));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void FirstOrDefault_InsidePredicate_Valid()
        {
            var result = students.FirstOrDefault(x => x.Age > 15);
            var mockResult = MockData.Students.FirstOrDefault(x => x.Age > 15);

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void FirstOrDefault_InsidePredicate_InValid()
        {
            var result = students.FirstOrDefault(x => x.Age > 100);
            var mockResult = MockData.Students.FirstOrDefault(x => x.Age > 100);

            Assert.Equal(mockResult, result);
        }
    }
}