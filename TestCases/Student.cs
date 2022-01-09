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
    }
}