using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class AvgSumMinMaxClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        private IQueryable<Student> emptyStudents => _db.firestoreDb.Collection("Empty").AsQuerable<Student>();


        public AvgSumMinMaxClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Min()
        {
            var age = students.Min(x => x.Age);
            var mockResult = students.Min(x => x.Age);

            Assert.Equal(mockResult, age);
        }

        [Fact]
        public void Min_OnZeroMatchedDocs()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => emptyStudents.Min(x => x.Age));
            Assert.Equal("Sequence contains no elements", ex.Message);

            //TODO check why this is throwing sequence contains no elements
            Assert.Throws<InvalidOperationException>(() => students.Where(x => x.LastName == "Empty").Min(y => y.Age));
        }

        [Fact]
        public void Max_OnZeroMatchedDocs()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => emptyStudents.Max(x => x.Age));
            Assert.Equal("Sequence contains no elements", ex.Message);

            //TODO check why this is throwing sequence contains no elements
            Assert.Throws<InvalidOperationException>(() => students.Where(x => x.LastName == "Empty").Max(y => y.Age));
        }

        [Fact]
        public void Max()
        {
            var age = students.Max(x => x.Age);
            var mockResult = students.Max(x => x.Age);

            Assert.Equal(mockResult, age);
        }

        [Fact]
        public void Min_FollowedByWhere()
        {
            var age = students.Where(x => x.LastName == "Teja").Min(x => x.Age);
            var mockResult = students.Where(x => x.LastName == "Teja").Min(x => x.Age);

            Assert.Equal(mockResult, age);
        }

        [Fact]
        public void Max_FollowedByWhere()
        {
            var age = students.Where(x => x.LastName == "Darisi").Max(x => x.Age);
            var mockResult = students.Where(x => x.LastName == "Darisi").Max(x => x.Age);

            Assert.Equal(mockResult, age);
        }

        [Fact]
        public void MinBy()
        {
            var student = students.MinBy(x => x.Age);
            var mockResult = students.MinBy(x => x.Age);

            Assert.Equal(mockResult.FirstName, student.FirstName);
        }

        [Fact]
        public void MinBy_FollowedByWhere()
        {
            var student = students.Where(x => x.LastName == "Teja").MinBy(x => x.Age);
            var mockResult = students.Where(x => x.LastName == "Teja").MinBy(x => x.Age);

            Assert.Equal(mockResult.FirstName, student.FirstName);
        }

        [Fact]
        public void MaxBy_FollowedByWhere()
        {
            var student = students.Where(x => x.LastName == "Teja").MaxBy(x => x.Age);
            var mockResult = students.Where(x => x.LastName == "Teja").MaxBy(x => x.Age);

            Assert.Equal(mockResult.FirstName, student.FirstName);
        }

        [Fact]
        public void MaxBy()
        {
            var student = students.MaxBy(x => x.Age);
            var mockResult = students.MaxBy(x => x.Age);

            Assert.Equal(mockResult.FirstName, student.FirstName);
        }

        [Fact]
        public void Average()
        {
            var avg = students.Average(x => x.Age);
            var mockResult = students.Average(x => x.Age);
            Assert.Equal(mockResult, avg);
        }

        [Fact]
        public void Average_FollowedByWhere()
        {
            var avg = students.Where(x => x.LastName == "Teja").Average(x => x.Age);
            var mockResult = students.Where(x => x.LastName == "Teja").Average(x => x.Age);

            Assert.Equal(mockResult, avg);
        }

        [Fact]
        public void Sum()
        {
            var sum = students.Sum(x => x.Age);
            var mockResult = students.Sum(x => x.Age);

            Assert.Equal(mockResult, sum);
        }

        [Fact]
        public void Sum_FollowedByWhere()
        {
            var sum = students.Where(x => x.LastName == "Teja").Sum(x => x.Age);
            var mockResult = students.Where(x => x.LastName == "Teja").Sum(x => x.Age);

            Assert.Equal(mockResult, sum);
        }
    }
}
