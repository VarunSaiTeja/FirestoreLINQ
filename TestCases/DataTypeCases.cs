using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class DataTypeCases : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<DataType> types => _db.firestoreDb.Collection("Types").AsQuerable<DataType>();

        public DataTypeCases(MyDb db)
        {
            _db = db;
        }

        DataType dataTypes = new()
        {
            Double = 77578.55,
            Float = 1.056f,
            String = "Hey",
            LatLng = new Google.Type.LatLng() { Latitude = -45, Longitude = 120 },
            Bool = true
        };

        [Fact]
        public void MockData()
        {
            var rs = _db.firestoreDb.Collection("Types").Document("Test").SetAsync(dataTypes).Result;
            Assert.NotNull(rs);
        }

        [Fact]
        public void GetEntireType()
        {
            var result = types.First();
            Assert.Equal(dataTypes, result);
        }
    }
}
