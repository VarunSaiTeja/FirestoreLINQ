using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class CountAnyClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        private IQueryable<Student> emptyStudents => _db.firestoreDb.Collection("Empty").AsQuerable<Student>();


        public CountAnyClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Count_OnValidCollection()
        {
            var count = students.Count();
            Assert.Equal(3, count);
        }

        [Fact]
        public void Count_OnInValidCollection()
        {
            var count = emptyStudents.Count();
            Assert.Equal(0, count);
        }

        [Fact]
        public void Any_OnValidCollection()
        {
            var any = students.Any();
            Assert.True(any);
        }

        [Fact]
        public void Any_OnInValidCollection()
        {
            var any = emptyStudents.Any();
            Assert.False(any);
        }

        [Fact]
        public void Where_AnyTrue()
        {
            var any = students.Where(x => x.Age > 25).Any();
            Assert.True(any);
        }

        [Fact]
        public void Where_AnyFalse()
        {
            var any = students.Where(x => x.Age > 100).Any();
            Assert.False(any);
        }

        [Fact]
        public void Where_Count()
        {
            var count = students.Where(x => x.Age >= 25).Count();
            Assert.Equal(2, count);
        }

        [Fact]
        public void Any_InsidePredicate_Case_Valid()
        {
            var result = students.Any(x => x.Age > 100);
            Assert.False(result);
        }

        [Fact]
        public void Any_InsidePredicate_Case_InValid()
        {
            var result = students.Any(x => x.Age > 25);
            Assert.True(result);
        }

        [Fact]
        public void Count_InsidePredicate_Case_1()
        {
            var count = students.Count(x => x.Age < 25);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Count_InsidePredicate_Case_2()
        {
            var count = students.Count(x => x.Age >= 25);
            Assert.Equal(2, count);
        }
    }
}
