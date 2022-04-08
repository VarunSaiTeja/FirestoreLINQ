using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class TakeSkipClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        public TakeSkipClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Take()
        {
            var results = students.Take(2).ToList();
            var mockResults = MockData.Students.Take(2).ToList();

            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void Take_Followedby_OrderBy()
        {
            var results = students.OrderBy(x => x.Age).Take(2).ToList();
            var mockResults = MockData.Students.OrderBy(x => x.Age).Take(2).ToList();

            Assert.Equal(mockResults.Count, results.Count);

            Assert.Equal(mockResults, results);
            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void Take_WhereBy()
        {
            var results = students.Take(1).Where(x => x.LastName == "Teja").ToList();
            var mockResults = MockData.Students.Take(1).Where(x => x.LastName == "Teja").ToList();

            Assert.Single(mockResults);
            Assert.Single(results);

            Assert.Equal(mockResults, results);
            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void Skip()
        {
            var results = students.Skip(1).ToList();
            var mockResults = MockData.Students.Skip(1).ToList();

            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void Skip_Followedby_OrderBy()
        {
            var results = students.OrderBy(x => x.Age).Skip(2).ToList();
            var mockResults = MockData.Students.OrderBy(x => x.Age).Skip(2).ToList();

            Assert.Single(mockResults);
            Assert.Single(results);

            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void Skip_WhereBy()
        {
            var results = students.Skip(1).Where(x => x.LastName == "Teja").ToList();
            var mockResults = MockData.Students.Skip(1).Where(x => x.LastName == "Teja").ToList();

            Assert.Single(mockResults);
            Assert.Single(results);

            Assert.Equal(mockResults, results);
            Assert.Equal(mockResults, results);
        }

        [Fact]
        public void SkipTake()
        {
            var results = students.Skip(1).Take(1).ToList();
            var mockResults = MockData.Students.Skip(1).Take(1).ToList();

            Assert.Single(mockResults);
            Assert.Single(results);

            Assert.Equal(mockResults, results);
        }
    }
}
