using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class SingleClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.Collection("Students").AsQuerable<Student>();
        private IQueryable<Student> emptyStudents => _db.firestoreDb.Collection("Empty").AsQuerable<Student>();

        public SingleClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Single_NoItems()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => emptyStudents.Single());
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void SingleOrDefault_NoItems()
        {
            Assert.Null(emptyStudents.SingleOrDefault());
        }

        [Fact]
        public void Single_MultipleItems()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.Single());
            Assert.Equal("Sequence contains more than one element", ex.Message);
        }

        [Fact]
        public void SingleOrDefault_MultipleItems()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.SingleOrDefault());
            Assert.Equal("Sequence contains more than one element", ex.Message);
        }

        [Fact]
        public void Single_Predicate_OneItem()
        {
            var result = students.Single(x => x.Age == 25);
            Assert.Equal("Sai", result.FirstName);
        }

        [Fact]
        public void SingleOrDefault_Predicate_OneItem()
        {
            var result = students.SingleOrDefault(x => x.Age == 25);
            Assert.Equal("Sai", result.FirstName);
        }

        [Fact]
        public void Single_Predicate_MultipleItems()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.Single(x => x.LastName == "Teja"));
            Assert.Equal("Sequence contains more than one element", ex.Message);
        }

        [Fact]
        public void SingleOrDefault_Predicate_MultipleItems()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.SingleOrDefault(x => x.LastName == "Teja"));
            Assert.Equal("Sequence contains more than one element", ex.Message);
        }

        [Fact]
        public void Single_Predicate_NoItem()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.Single(x => x.Age > 100));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void SingleOrDefault_Predicate_NoItem()
        {
            Assert.Null(students.SingleOrDefault(x => x.Age > 100));
        }
    }
}
