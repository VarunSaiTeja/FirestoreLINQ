using Google.Cloud.Firestore;
using System.Collections.Generic;
namespace TestCases
{
    [FirestoreData]
    public class Student
    {
        [FirestoreProperty]
        public List<string> Skills { get; set; }

        [FirestoreProperty]
        public string FirstName { get; set; }

        [FirestoreProperty]
        public string LastName { get; set; }

        [FirestoreProperty]
        public int Age { get; set; }

        [FirestoreProperty]
        public Address Address { get; set; }
    }

    [FirestoreData]
    public class Address
    {
        [FirestoreProperty]
        public string City { get; set; }

        [FirestoreProperty]
        public string Country { get; set; }
    }
}