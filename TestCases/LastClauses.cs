using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class LastClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.Collection("Students").AsQuerable<Student>();

        public LastClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Last_WithoutOrderBy()
        {
            var ex = Assert.Throws<AggregateException>(() => students.Last());
            var innerEx = ex.InnerExceptions.First();

            Assert.Equal("InvalidOperationException", innerEx.GetType().Name);
            Assert.Equal("Queries using LimitToLast must specify at least one ordering.", innerEx.Message);
        }

        [Fact]
        public void Last_WithOrderBy()
        {
            var results = students.OrderBy(x => x.FirstName).Last();
            Assert.Equal("Varun", results.FirstName);
        }

        [Fact]
        public void Last_InsidePredicate_Valid()
        {
            var result = students.OrderBy(x => x.Age).Last(x => x.Age > 15);
            Assert.Equal("Varun", result.FirstName);
        }

        [Fact]
        public void Last_InsidePredicate_InValid()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.OrderBy(x => x.Age).Last(x => x.Age > 100));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void LastOrDefault_InsidePredicate_Valid()
        {
            var result = students.OrderBy(x => x.Age).LastOrDefault(x => x.Age > 15);
            Assert.Equal("Varun", result.FirstName);
        }

        [Fact]
        public void LastOrDefault_InsidePredicate_InValid()
        {
            var result = students.OrderBy(x => x.Age).LastOrDefault(x => x.Age > 100);
            Assert.Null(result);
        }
    }
}