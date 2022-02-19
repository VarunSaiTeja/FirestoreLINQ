using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class FirestoreCollection : IClassFixture<MyDb>
    {
        private readonly MyDb _db;
        public FirestoreCollection(MyDb db)
        {
            _db = db;
        }
        [Fact]
        public void GetAsQuerable()
        {
            var results = _db.firestoreDb.AsQuerable<Student>().ToList();
            Assert.Equal(3, results.Count);
        }
    }
}
