using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class MockDataSetup : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<DataType> types => _db.firestoreDb.AsQuerable<DataType>();

        public MockDataSetup(MyDb db)
        {
            _db = db;
        }

        [Fact(Skip = "Use this test only for mocking/inserting Types data in firestore, Run this test alone intially.")]
        public void Setup_Types()
        {
            var rs = _db.firestoreDb.Collection<DataType>().Document("Test").SetAsync(DataTypeCases.dataTypes).Result;
            Assert.NotNull(rs);
        }

        [Fact(Skip = "Use this test only for mocking/inserting Students data in firestore, Run this test alone intially.")]
        public void Setup_Students()
        {
            Console.WriteLine(MockData.Students.Count);
            foreach (var student in MockData.Students)
            {
                var rs = _db.firestoreDb.Collection<Student>().AddAsync(student).Result;
                Assert.NotNull(rs);
            }
        }
    }
}
