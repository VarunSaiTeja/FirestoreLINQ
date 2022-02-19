using Google.Cloud.Firestore;
using Google.Type;
using System;
using FirestoreLINQ;

namespace TestCases
{
    [FirestoreData,FirestoreCollection("Types")]
    public class DataType : IEquatable<DataType>
    {
        [FirestoreProperty]
        public double Double { get; set; }

        [FirestoreProperty]
        public float Float { get; set; }

        [FirestoreProperty]
        public string String { get; set; }

        [FirestoreProperty]
        public LatLng LatLng { get; set; }

        [FirestoreProperty]
        public bool Bool { get; set; }

        public bool Equals(DataType other)
        {
            var floatingPoints = other.Float == Float && other.Double == Double;
            var text = other.String == String;
            var point = other.LatLng.Latitude == LatLng.Latitude && other.LatLng.Longitude == LatLng.Longitude;
            var boolean = other.Bool == Bool;

            return floatingPoints && text && point && boolean;
        }
    }
}
