using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class TakeSkipClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.Collection("Students").AsQuerable<Student>();

        public TakeSkipClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Take()
        {
            var results = students.Take(2).ToList();
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Take_Followedby_OrderBy()
        {
            var results = students.OrderBy(x => x.Age).Take(2).ToList();
            Assert.Equal(2, results.Count);

            Assert.Equal("Harshini", results[0].FirstName);
            Assert.Equal("Sai", results[1].FirstName);
        }

        [Fact]
        public void Take_WhereBy()
        {
            var results = students.Take(1).Where(x => x.LastName == "Teja").ToList();
            Assert.Single(results);
            Assert.Equal("Teja", results[0].LastName);
            Assert.Equal("Sai", results[0].FirstName);
        }

        [Fact]
        public void Skip()
        {
            var results = students.Skip(1).ToList();
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Skip_Followedby_OrderBy()
        {
            var results = students.OrderBy(x => x.Age).Skip(2).ToList();
            Assert.Single(results);

            Assert.Equal("Varun", results[0].FirstName);
        }

        [Fact]
        public void Skip_WhereBy()
        {
            var results = students.Skip(1).Where(x => x.LastName == "Teja").ToList();
            Assert.Single(results);
            Assert.Equal("Teja", results[0].LastName);
            Assert.Equal("Varun", results[0].FirstName);
        }

        [Fact]
        public void SkipTake()
        {
            var results = students.Skip(1).Take(1).ToList();
            Assert.Single(results);
            Assert.Equal("Varun", results[0].FirstName);
        }
    }
}
