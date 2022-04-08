using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
namespace TestCases
{
    [FirestoreData]
    public class Student: IEquatable<Student>
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

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

        public bool Equals(Student other)
        {
            return this.Id==other.Id &&
                this.FirstName==other.FirstName &&
                this.LastName==other.LastName &&
                this.Age==other.Age;
        }
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