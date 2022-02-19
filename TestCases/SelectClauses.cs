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
            Assert.Equal(3, firstNames.Count);
            Assert.Equal("Sai", firstNames[0]);
            Assert.Equal("Varun", firstNames[1]);
            Assert.Equal("Harshini", firstNames[2]);
        }

        [Fact]
        public void SelectMultipleField_SamePropertyName()
        {
            var names = students.Select(x => new { x.FirstName, x.LastName }).ToList();
            Assert.Equal(3, names.Count);
            Assert.Equal("Sai", names[0].FirstName);
            Assert.Equal("Varun", names[1].FirstName);
            Assert.Equal("Harshini", names[2].FirstName);
            Assert.Equal("Teja", names[0].LastName);
            Assert.Equal("Teja", names[1].LastName);
            Assert.Equal("Darisi", names[2].LastName);
        }

        [Fact]
        public void SelectMultipleField_DifferentPropertyName()
        {
            var names = students.Select(x => new { FN = x.FirstName, LN = x.LastName }).ToList();
            Assert.Equal(3, names.Count);
            Assert.Equal("Sai", names[0].FN);
            Assert.Equal("Varun", names[1].FN);
            Assert.Equal("Harshini", names[2].FN);
            Assert.Equal("Teja", names[0].LN);
            Assert.Equal("Teja", names[1].LN);
            Assert.Equal("Darisi", names[2].LN);
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
