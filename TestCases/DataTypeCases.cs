using FirestoreLINQ;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class DataTypeCases : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<DataType> types => _db.firestoreDb.AsQuerable<DataType>();

        public DataTypeCases(MyDb db)
        {
            _db = db;
        }

        public static DataType dataTypes = new()
        {
            Double = 77578.55,
            Float = 1.056f,
            String = "Hey",
            LatLng = new Google.Type.LatLng() { Latitude = -45, Longitude = 120 },
            Bool = true
        };

        [Fact]
        public void GetEntireType()
        {
            var result = types.First();
            Assert.Equal(dataTypes, result);
        }
    }
}
