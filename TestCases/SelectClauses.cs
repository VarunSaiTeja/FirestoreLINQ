using FirestoreLINQ;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace TestCases
{
    public class SelectClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        public SelectClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void SelectSingleField()
        {
            var firstNames = students.Select(x => x.FirstName).ToList();
            var mockResult=MockData.Students.Select(x => x.FirstName).ToList();

            Assert.Equal(mockResult, firstNames);
        }

        [Fact]
        public void SelectMultipleField_SamePropertyName()
        {
            var names = students.Select(x => new { x.FirstName, x.LastName }).ToList();
            var mockResult = MockData.Students.Select(x => new { x.FirstName, x.LastName }).ToList();

            Assert.Equal(mockResult, names);
        }

        [Fact]
        public void SelectMultipleField_DifferentPropertyName()
        {
            var names = students.Select(x => new { FN = x.FirstName, LN = x.LastName }).ToList();
            var mockResult = MockData.Students.Select(x => new { FN = x.FirstName, LN = x.LastName }).ToList();
            
            Assert.Equal(mockResult, names);
        }

        [Fact]
        public void Select_WithFieldAggregation_ThrowsEx()
        {
            Assert.Throws<TargetInvocationException>(() => students.Select(x => x.FirstName + " " + x.LastName).ToList());
        }

        [Fact]
        public void Where_FollowedBy_SingleFieldSelect_ThrowsEx()
        {
            var ex = Assert.Throws<NotSupportedException>(() => students.Select(x => x.LastName).Where(x => x == "Teja").Count());
            Assert.Equal("Passing paramater is not supported", ex.Message);
        }
    }
}
