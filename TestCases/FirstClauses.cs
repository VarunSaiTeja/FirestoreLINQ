using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class FirstClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.Collection("Students").AsQuerable<Student>();

        public FirstClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void First()
        {
            var results = students.First();
            Assert.Equal("Sai", results.FirstName);
        }

        [Fact]
        public void OrderBy_First()
        {
            var results = students.OrderBy(x => x.FirstName).First();
            Assert.Equal("Harshini", results.FirstName);
        }

        [Fact]
        public void First_InsidePredicate_Valid()
        {
            var result = students.First(x => x.Age > 15);
            Assert.Equal("Sai", result.FirstName);
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
            Assert.Equal("Sai", result.FirstName);
        }

        [Fact]
        public void FirstOrDefault_InsidePredicate_InValid()
        {
            var result = students.FirstOrDefault(x => x.Age > 100);
            Assert.Null(result);
        }
    }
}